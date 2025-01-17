/*  This file is part of WillowTree#
 * 
 *  Copyright (C) 2011 Matthew Carter <matt911@users.sf.net>
 *  Copyright (C) 2010, 2011 XanderChaos
 *  Copyright (C) 2011 Thomas Kaiser
 *  Copyright (C) 2010 JackSchitt
 * 
 *  WillowTree# is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  WillowTree# is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with WillowTree#.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X360.IO;
using X360.STFS;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace WillowTree
{
    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

    public class WillowSaveGame
    {
        // Multiple users using Parallels on Mac reported that WT# was crashing at startup.
        // I narrowed it down to this line.  Application.get_ExecutablePath() is giving them 
        // a UriFormatException for some reason.  I'm rewriting this line to try to resolve it.
        // This also removes the dependence of WillowSaveGame on Windows Forms since
        // Application is a part of the System.Windows.Forms namespace.
        //public static string AppPath = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar;
        public static string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
        public static string DataPath = AppPath + "Data" + Path.DirectorySeparatorChar;

        public int SaveValuesCount;             // The number of values that must be stored in the savegame for each weapon/item
        public const int ExportValuesCount = 4; // The number of values that will be exported to clipboard from each weapon/item

        // Used for all single-byte string encodings.
        private static Encoding _singleByteEncoding; // DO NOT REFERENCE THIS DIRECTLY!
        private static Encoding SingleByteEncoding
        {
            get
            {
                // Not really thread safe, but it doesn't matter (Encoding
                // should be effectively sealed).
                if (_singleByteEncoding == null)
                {
                    // Use ISO 8859-1 (Windows 1252) encoding for all single-
                    // byte strings.
                    _singleByteEncoding = Encoding.GetEncoding(1252);
                    System.Diagnostics.Debug.Assert(_singleByteEncoding != null,
                        "singleByteEncoding != null");
                    System.Diagnostics.Debug.Assert(_singleByteEncoding.IsSingleByte,
                        "Given string encoding is not a single-byte encoding.");
                }

                return _singleByteEncoding;
            }
        }

        private static byte[] ReadBytes(BinaryReader br, int fieldSize, ByteOrder byteOrder)
        {
            byte[] bytes = br.ReadBytes(fieldSize);
            if (bytes.Length != fieldSize)
                throw new EndOfStreamException();

            if (BitConverter.IsLittleEndian)
            {
                if (byteOrder == ByteOrder.BigEndian)
                    Array.Reverse(bytes);
            }
            else
            {
                if (byteOrder == ByteOrder.LittleEndian)
                    Array.Reverse(bytes);
            }

            return bytes;
        }
        private static byte[] ReadBytes(byte[] inBytes, int fieldSize, ByteOrder byteOrder)
        {
            System.Diagnostics.Debug.Assert(inBytes != null, "inBytes != null");
            System.Diagnostics.Debug.Assert(inBytes.Length >= fieldSize, "inBytes.Length >= fieldSize");

            byte[] outBytes = new byte[fieldSize];
            Buffer.BlockCopy(inBytes, 0, outBytes, 0, fieldSize);

            if (BitConverter.IsLittleEndian)
            {
                if (byteOrder == ByteOrder.BigEndian)
                    Array.Reverse(outBytes, 0, fieldSize);
            }
            else
            {
                if (byteOrder == ByteOrder.LittleEndian)
                    Array.Reverse(outBytes, 0, fieldSize);
            }

            return outBytes;
        }

        private static float ReadSingle(BinaryReader reader, ByteOrder Endian)
        {
            return BitConverter.ToSingle(ReadBytes(reader, sizeof(float), Endian), 0);
        }
        private static int ReadInt32(BinaryReader reader, ByteOrder Endian)
        {
            return BitConverter.ToInt32(ReadBytes(reader, sizeof(int), Endian), 0);
        }
        private static short ReadInt16(BinaryReader reader, ByteOrder Endian)
        {
            return BitConverter.ToInt16(ReadBytes(reader, sizeof(short), Endian), 0);
        }
        private static List<int> ReadListInt32(BinaryReader reader, ByteOrder Endian)
        {
            int count = ReadInt32(reader, Endian);
            List<int> list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int value = ReadInt32(reader, Endian);
                list.Add(value);
            }
            return list;
        }

        private static void Write(BinaryWriter writer, float value, ByteOrder endian)
        {
            writer.Write(BitConverter.ToSingle(ReadBytes(BitConverter.GetBytes(value), sizeof(float), endian), 0));
        }
        private static void Write(BinaryWriter writer, int value, ByteOrder endian)
        {
            writer.Write(BitConverter.ToInt32(ReadBytes(BitConverter.GetBytes(value), sizeof(int), endian), 0));
        }
        private static void Write(BinaryWriter writer, short value, ByteOrder Endian)
        {
            writer.Write(ReadBytes(BitConverter.GetBytes((short)value), sizeof(short), Endian));
        }

        ///<summary>Reads a string in the format used by the WSGs</summary>
        private static string ReadString(BinaryReader reader, ByteOrder endian)
        {
            int tempLengthValue = ReadInt32(reader, endian);
            if (tempLengthValue == 0)
                return string.Empty;

            string value;

            // matt911: Borderlands doesn't ever use unicode strings as far
            // as I can tell.  All text seems to be single-byte encoded with a code
            // page for the current culture so tempLengthValue is always positive.
            //
            // da_fileserver implemented the unicode string reading to agree with the 
            // way that unreal engine 3 uses it I think, but I can't test this code 
            // because I know of no place where Borderlands itself actually uses it.
            //
            // It appears to me that ReadString and WriteString are filled with a lot of
            // unnecessary code to deal with unicode, but since the code is already 
            // implemented and I haven't had any problems I'd rather leave in code that
            // is not necessary than break something that works already
            
            // Read string data (either single-byte or Unicode).
            if (tempLengthValue < 0)
            {
                // Convert the length value into a unicode byte count.
                tempLengthValue = -tempLengthValue * 2;

                // If the string length is over 4K assume that the string is invalid.
                // This prevents an out of memory exception in the case of invalid data.
                if (tempLengthValue > 4096)
                    throw new InvalidDataException("String length was too long.");

                // Read the byte data (and ensure that the number of bytes
                // read matches the number of bytes it was supposed to read--
                // BinaryReader may not return the same number of bytes read).
                byte[] data = reader.ReadBytes(tempLengthValue);
                if (data.Length != tempLengthValue)
                    throw new EndOfStreamException();

                // Convert the byte data into a string.
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                // If the string length is over 4K assume that the string is invalid.
                // This prevents an out of memory exception in the case of invalid data.
                if (tempLengthValue > 4096)
                    throw new InvalidDataException("String length was too long.");

                // Read the byte data (and ensure that the number of bytes
                // read matches the number of bytes it was supposed to read--
                // BinaryReader may not return the same number of bytes read).
                byte[] data = reader.ReadBytes(tempLengthValue);
                if (data.Length != tempLengthValue)
                    throw new EndOfStreamException();

                // Convert the byte data into a string.
                value = SingleByteEncoding.GetString(data);
            }

            // Look for the null terminator character. If not found then then string is 
            // probably corrupt.  
            int nullTerminatorIndex = value.IndexOf('\0');
            if (nullTerminatorIndex != value.Length - 1)
                throw new InvalidDataException("String was not properly terminated with a null character.");

            // Return the string, excluding the null terminator
            return value.Substring(0, nullTerminatorIndex);
        }
        private static void Write(BinaryWriter writer, string value, ByteOrder endian)
        {
            // Null and empty strings are treated the same (with an output
            // length of zero).
            if (string.IsNullOrEmpty(value))
            {
                writer.Write(0);
                return;
            }

            bool requiresUnicode = isUnicode(value);
            // Generate the bytes (either single-byte or Unicode, depending on input).
            if (!requiresUnicode)
            {
                // Write character length (including null terminator).
                Write(writer, value.Length + 1, endian);

                // Write single-byte encoded string.
                writer.Write(SingleByteEncoding.GetBytes(value));

                // Write null terminator.
                writer.Write((byte)0);
            }
            else
            {
                // Write character length (including null terminator).
                Write(writer, -1 - value.Length, endian);

                // Write UTF-16 encoded string.
                writer.Write(Encoding.Unicode.GetBytes(value));

                // Write null terminator.
                writer.Write((short)0);
            }
        }
        /// <summary> Look for any non-ASCII characters in the input.</summary>
        private static bool isUnicode(string value)
        {
            for (int i = 0; i < value.Length; i++)
                if (value[i] > 256)
                    return true;

            return false;
        }

        #region Members
        public ByteOrder EndianWSG;

        public string Platform;
        public string OpenedWSG;
        public bool ContainsRawData;
        // Whether WSG should try to automatically repair or discard any invalid data
        // to recover from an invalid state.  This will allow partial data loss but 
        // may allow partial data recovery as well.
        public bool AutoRepair = false;
        public bool RequiredRepair;

        //General Info
        public string MagicHeader;
        public int VersionNumber;
        public string PLYR;
        public int RevisionNumber;
        public string Class;
        public int Level;
        public int Experience;
        public int SkillPoints;
        public int Unknown1;
        public int Cash;
        public int FinishedPlaythrough1;

        //Skill Arrays
        public int NumberOfSkills;
        public string[] SkillNames;
        public int[] LevelOfSkills;
        public int[] ExpOfSkills;
        public int[] InUse;

        //Vehicle Info
        public int Vehi1Color;
        public int Vehi2Color;
        public int Vehi1Type; // 0 = rocket, 1 = machine gun
        public int Vehi2Type;

        //Ammo Pool Arrays
        public int NumberOfPools;
        public string[] ResourcePools;
        public string[] AmmoPools;
        public float[] RemainingPools;
        public int[] PoolLevels;

        //Item Arrays
        public int NumberOfItems;
        public List<List<string>> ItemStrings = new List<List<string>>();
        public List<List<int>> ItemValues = new List<List<int>>();

        //Backpack Info
        public int BackpackSize;
        public int EquipSlots;

        //Weapon Arrays
        public int NumberOfWeapons;
        public List<List<string>> WeaponStrings = new List<List<string>>();
        public List<List<int>> WeaponValues = new List<List<int>>();

        //Challenge related data
        public int NumberOfChallenges;
        public int ChallengeDataBlockLength;
        public int ChallengeDataBlockId;
        public int ChallengeDataLength;
        public Int16 ChallengeDataEntries;
        public struct ChallengeDataEntry
        {
            public Byte Id;
            public Int16 Static;
            public Int32 Value;
            public string Name;
            public string Description;
            public string Level1;
            public string Value1;
            public string Level2;
            public string Value2;
            public string Level3;
            public string Value3;
            public string Level4;
            public string Value4;
        }
        public List<ChallengeDataEntry> Challenges;
        public byte[] ChallengeData;

        //Location related data
        public int TotalLocations;
        public string[] LocationStrings;
        public string CurrentLocation;
        public int[] SaveInfo1to5 = new int[5];
        public int SaveNumber;
        public int[] SaveInfo7to10 = new int[4];

        public struct QuestObjective
        {
            public int Progress;
            public string Description;
        }

        public class QuestEntry
        {
            public string Name;
            public int Progress;
            public int DLCValue1;
            public int DLCValue2;
            public int NumberOfObjectives;
            public QuestObjective[] Objectives;
        }

        public class QuestTable
        { 
            public List<QuestEntry> Quests;
            public int Index;
            public string CurrentQuest;
            public int TotalQuests;
        }

        //Quest Arrays/Info
        //public class QuestTable
        //{
        //    public int Index;
        //    public string CurrentQuest;
        //    public int TotalQuests;
        //    public string[] QuestStrings;
        //    public int[,] QuestValues;
        //    public string[,] QuestSubfolders;
        //};

        public int NumberOfQuestLists;
        public List<QuestTable> QuestLists = new List<QuestTable>();

        //More unknowns and color info.
        public int TotalPlayTime;
        public string LastPlayedDate;
        public string CharacterName;
        public int Color1;
        public int Color2;
        public int Color3;
        public byte[] EnhancedBlock;
        public byte[] EnhancedBlock2;
        public int Unknown2;
        public List<int> PromoCodesUsed;
        public List<int> PromoCodesRequiringNotification;

        //Echo Info
        public int NumberOfEchoLists;
        public class EchoEntry
        {
            public string Name;
            public int DLCValue1;
            public int DLCValue2;
        }
        public class EchoTable
        {
            public int Index;
            public int TotalEchoes;
            public List<EchoEntry> Echoes;
        };
        public List<EchoTable> EchoLists = new List<EchoTable>();

        // Temporary lists used for primary pack data when the inventory is split
        public List<List<string>> ItemStrings1;
        public List<List<int>> ItemValues1;
        public List<List<string>> WeaponStrings1;
        public List<List<int>> WeaponValues1;
        // Temporary lists used for secondary pack data when the inventory is split
        public List<List<string>> ItemStrings2;
        public List<List<int>> ItemValues2;
        public List<List<string>> WeaponStrings2;
        public List<List<int>> WeaponValues2;

        public class DLC_Data
        {
            public const int Section1Id = 0x43211234;
            public const int Section2Id = 0x02151984;
            public const int Section3Id = 0x32235947;
            public const int Section4Id = 0x234BA901;
            public const uint Section5Id = 0xff052012;

            public bool HasSection1;
            public bool HasSection2;
            public bool HasSection3;
            public bool HasSection4;

            public List<DLCSection> DataSections;

            public int DLC_Size;

            // DLC Section 1 Data (bank data)
            public byte DLC_Unknown1;  // Read only flag. Always resets to 1 in ver 1.41.  Probably CanAccessBank.
            public int BankSize;
            public List<BankEntry> BankInventory = new List<BankEntry>();
            // DLC Section 2 Data (don't know)
            public int DLC_Unknown2; // All four of these are boolean flags.
            public int DLC_Unknown3; // If you set them to any value except 0
            public int DLC_Unknown4; // the game will rewrite them as 1.
            public int SkipDLC2Intro; //
            // DLC Section 3 Data (related to the level cap.  removing this section will delevel your character to 50)
            public byte DLC_Unknown5;  // Read only flag. Always resets to 1 in ver 1.41.  Probably CanExceedLevel50
            // DLC Section 4 Data (DLC backpack)
            public byte SecondaryPackEnabled;  // Read only flag. Always resets to 1 in ver 1.41.
            public int NumberOfItems;
            public List<List<string>> ItemStrings = new List<List<string>>();
            public List<List<int>> ItemValues = new List<List<int>>();
            public int NumberOfWeapons;
            public List<List<string>> WeaponStrings = new List<List<string>>();
            public List<List<int>> WeaponValues = new List<List<int>>();
        }

        public DLC_Data DLC = new DLC_Data();

        public class DLCSection
        {
            public int Id;
            public byte[] RawData;
            public byte[] BaseData; // used temporarily in SaveWSG to store the base data for a section as a byte array
        }

        //Xbox 360 only
        public long ProfileID;
        public byte[] DeviceID;
        public byte[] CON_Image;
        public string Title_Display;
        public string Title_Package;
        public uint TitleID = 1414793191;
        #endregion


        public static uint GetXboxTitleID(Stream inputWSG)
        {
            BinaryReader br = new BinaryReader(inputWSG);
            byte[] fileInMemory = br.ReadBytes((int)inputWSG.Length);
            if (fileInMemory.Count() != inputWSG.Length)
                throw new EndOfStreamException();

            try
            {
                STFSPackage CON = new STFSPackage(new DJsIO(fileInMemory, true), new X360.Other.LogRecord());
                return CON.Header.TitleID;
            }
            catch { return 0; }
        }

        ///<summary>Reports back the expected platform this WSG was created on.</summary>
        public static string WSGType(Stream inputWSG)
        {
            BinaryReader saveReader = new BinaryReader(inputWSG);

            byte byte1 = saveReader.ReadByte();
            byte byte2 = saveReader.ReadByte();
            byte byte3 = saveReader.ReadByte();
            // These bytes represent the characters CON in windows code page 1252
            if (byte1 == 0x43 && byte2 == 0x4f && byte3 == 0x4e)
            {
                byte byte4 = saveReader.ReadByte();
                // This byte represents the space character ' ' in windows code page 1252
                if (byte4 == 0x20)
                {
                    // This is a really lame way to check for the WSG data...
                    saveReader.BaseStream.Seek(53244, SeekOrigin.Current);

                    byte1 = saveReader.ReadByte();
                    byte2 = saveReader.ReadByte();
                    byte3 = saveReader.ReadByte();
                    // These bytes represet the characters WSG in Windows code page 1252
                    if (byte1 == 0x57 && byte2 == 0x53 && byte3 == 0x47)
                    {
                        saveReader.BaseStream.Seek(0x360, SeekOrigin.Begin);
                        uint titleID = ((uint)saveReader.ReadByte() << 24) + ((uint)saveReader.ReadByte() << 16) +
                            ((uint)saveReader.ReadByte() << 8) + (uint)saveReader.ReadByte();


//                        uint titleID = GetXboxTitleID(inputWSG);
                        switch (titleID) 
                        {
                            case 1414793191: return "X360";
                            case 1414793318: return "X360JP";
                            default: return "unknown";
                        }
                    }
                }
            }
            else if (byte1 == 0x57 && byte2 == 0x53 && byte3 == 0x47)
            {
                int wsgVersion = saveReader.ReadInt32();

                // BinaryReader.ReadInt32 always uses little-endian byte order.
                bool littleEndian;
                switch (wsgVersion)
                {
                    case 0x02000000: // 33554432 decimal
                        littleEndian = false;
                        break;
                    case 0x00000002:
                        littleEndian = true;
                        break;
                    default:
                        return "unknown";
                }

                if (littleEndian)
                    return "PC";
                else
                    return "PS3";
            }

            return "Not WSG";
        }

        ///<summary>Extracts a WSG from a CON (Xbox 360 Container File).</summary>
        public MemoryStream OpenXboxWSGStream(Stream InputX360File)
        {
            BinaryReader br = new BinaryReader(InputX360File);
            byte[] fileInMemory = br.ReadBytes((int)InputX360File.Length);
            if (fileInMemory.Count() != InputX360File.Length)
                throw new EndOfStreamException();

            try
            {
                STFSPackage CON = new STFSPackage(new DJsIO(fileInMemory, true), new X360.Other.LogRecord());
                //DJsIO Extract = new DJsIO(true);
                //CON.FileDirectory[0].Extract(Extract);
                ProfileID = CON.Header.ProfileID;
                DeviceID = CON.Header.DeviceID;
                
                //DJsIO Save = new DJsIO("C:\\temp.sav", DJFileMode.Create, true);
                //Save.Write(Extract.ReadStream());
                //Save.Close();
                //byte[] nom = CON.GetFile("SaveGame.sav").GetEntryData(); 
                return new MemoryStream(CON.GetFile("SaveGame.sav").GetTempIO(true).ReadStream(), false);
            }
            catch
            {
                try
                {
                    DJsIO Manual = new DJsIO(fileInMemory, true);
                    Manual.ReadBytes(881);
                    ProfileID = Manual.ReadInt64();
                    Manual.ReadBytes(132);
                    DeviceID = Manual.ReadBytes(20);
                    Manual.ReadBytes(48163);
                    int size = Manual.ReadInt32();
                    Manual.ReadBytes(4040);
                    return new MemoryStream(Manual.ReadBytes(size), false);
                }
                catch { return null; }
            }
        }

        ///<summary>Reads savegame data from a file</summary>
        public void LoadWSG(string inputFile)
        {
            using (FileStream fileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Platform = WSGType(fileStream);
                fileStream.Seek(0, SeekOrigin.Begin);

                if (string.Equals(Platform, "X360", StringComparison.Ordinal) ||
                    string.Equals(Platform, "X360JP", StringComparison.Ordinal))
                {
                    using (MemoryStream x360FileStream = OpenXboxWSGStream(fileStream))
                    {
                        ReadWSG(x360FileStream);
                    }
                }
                else if (string.Equals(Platform, "PS3", StringComparison.Ordinal) ||
                    string.Equals(Platform, "PC", StringComparison.Ordinal))
                {
                    ReadWSG(fileStream);
                }
                else
                {
                    throw new FileFormatException("Input file is not a WSG (platform is " + Platform + ").");
                }

                OpenedWSG = inputFile;
            }
        }

        private void BuildXboxPackage(string packageFileName, string saveFileName, int locale)
        {
            CreateSTFS Package = new CreateSTFS();

            Package.STFSType = STFSType.Type1;
            Package.HeaderData.ProfileID = this.ProfileID;
            Package.HeaderData.DeviceID = this.DeviceID;

            Assembly newAssembly = Assembly.GetExecutingAssembly();
            // WARNING: GetManifestResourceStream is case-sensitive.
            Stream WT_Icon = newAssembly.GetManifestResourceStream("WillowTree.Resources.WT_CON.png");
            Package.HeaderData.ContentImage = System.Drawing.Image.FromStream(WT_Icon);
            Package.HeaderData.PackageImage = Package.HeaderData.ContentImage;
            Package.HeaderData.Title_Display = this.CharacterName + " - Level " + this.Level + " - " + CurrentLocation;
            Package.HeaderData.Title_Package = "Borderlands";
            switch (locale)
            {
                case 1: // US or International version
                    Package.HeaderData.Title_Package = "Borderlands";
                    Package.HeaderData.TitleID = 1414793191;
                    break;                   
                case 2: // JP version
                    Package.HeaderData.Title_Package = "Borderlands (JP)";
                    Package.HeaderData.TitleID = 1414793318;
                    break;
            }
            Package.AddFile(saveFileName, "SaveGame.sav");


            STFSPackage CON = new STFSPackage(Package, new RSAParams(DataPath + "KV.bin"), packageFileName, new X360.Other.LogRecord());

            CON.FlushPackage(new RSAParams(DataPath + "KV.bin"));
            CON.CloseIO();
            WT_Icon.Close();
        }
        
        public void SaveWSG(string filename)
        {
            if (this.Platform == "PS3" || this.Platform == "PC")
            {
                using (BinaryWriter Save = new BinaryWriter(new FileStream(filename, FileMode.Create)))
                {
                    Save.Write(this.WriteWSG());
                }
            }

            else if (this.Platform == "X360")
            {
                string tempSaveName = filename + ".temp";
                using (BinaryWriter Save = new BinaryWriter(new FileStream(tempSaveName, FileMode.Create)))
                {
                    Save.Write(this.WriteWSG());
                }

                BuildXboxPackage(filename, tempSaveName, 1);
                File.Delete(tempSaveName);
            }

            else if (this.Platform == "X360JP")
            {
                string tempSaveName = filename + ".temp";
                using (BinaryWriter Save = new BinaryWriter(new FileStream(tempSaveName, FileMode.Create)))
                {
                    Save.Write(this.WriteWSG());
                }

                BuildXboxPackage(filename, tempSaveName, 2);
                File.Delete(tempSaveName);
            }

        }

        ///<summary>Read savegame data from an open stream</summary>
        public void ReadWSG(Stream fileStream)
        {
            BinaryReader TestReader = new BinaryReader(fileStream, Encoding.ASCII);

            ContainsRawData = false;
            RequiredRepair = false;
            MagicHeader = new string(TestReader.ReadChars(3));
            VersionNumber = TestReader.ReadInt32();

            if (VersionNumber == 2)
                EndianWSG = ByteOrder.LittleEndian;
            else if (VersionNumber == 0x02000000)
            {
                VersionNumber = 2;
                EndianWSG = ByteOrder.BigEndian;
            }
            else
                throw new FileFormatException("WSG version number does match any known version (" + VersionNumber + ").");
         
            PLYR = new string(TestReader.ReadChars(4));
            if (!string.Equals(PLYR, "PLYR", StringComparison.Ordinal))
                throw new FileFormatException("Player header does not match expected value.");

            RevisionNumber = ReadInt32(TestReader, EndianWSG);
            if (RevisionNumber >= 0x27)
                SaveValuesCount = 6;
            else
                SaveValuesCount = 4;

            Class = ReadString(TestReader, EndianWSG);
            Level = ReadInt32(TestReader, EndianWSG);
            Experience = ReadInt32(TestReader, EndianWSG);
            SkillPoints = ReadInt32(TestReader, EndianWSG);
            Unknown1 = ReadInt32(TestReader, EndianWSG);
            Cash = ReadInt32(TestReader, EndianWSG);
            FinishedPlaythrough1 = ReadInt32(TestReader, EndianWSG);
            NumberOfSkills = ReadSkills(TestReader, EndianWSG);
            Vehi1Color = ReadInt32(TestReader, EndianWSG);
            Vehi2Color = ReadInt32(TestReader, EndianWSG);
            Vehi1Type = ReadInt32(TestReader, EndianWSG);
            Vehi2Type = ReadInt32(TestReader, EndianWSG);
            NumberOfPools = ReadAmmo(TestReader, EndianWSG);
            ItemStrings = new List<List<string>>();
            ItemValues = new List<List<int>>();
            NumberOfItems = ReadItems(TestReader, EndianWSG);
            BackpackSize = ReadInt32(TestReader, EndianWSG);
            EquipSlots = ReadInt32(TestReader, EndianWSG);
            WeaponStrings = new List<List<string>>();
            WeaponValues = new List<List<int>>();
            NumberOfWeapons = ReadWeapons(TestReader, EndianWSG);
            NumberOfChallenges = ReadChallenges(TestReader, EndianWSG);
            TotalLocations = ReadLocations(TestReader, EndianWSG);
            CurrentLocation = ReadString(TestReader, EndianWSG);
            SaveInfo1to5[0] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[1] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[2] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[3] = ReadInt32(TestReader, EndianWSG);
            SaveInfo1to5[4] = ReadInt32(TestReader, EndianWSG);
            SaveNumber = ReadInt32(TestReader, EndianWSG);
            SaveInfo7to10[0] = ReadInt32(TestReader, EndianWSG);
            SaveInfo7to10[1] = ReadInt32(TestReader, EndianWSG);
            NumberOfQuestLists = ReadQuests(TestReader, EndianWSG);

            TotalPlayTime = ReadInt32(TestReader, EndianWSG);
            LastPlayedDate = ReadString(TestReader, EndianWSG); //YYYYMMDDHHMMSS
            CharacterName = ReadString(TestReader, EndianWSG);
            Color1 = ReadInt32(TestReader, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Color2 = ReadInt32(TestReader, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Color3 = ReadInt32(TestReader, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            if (RevisionNumber >= 0x27)
              EnhancedBlock = TestReader.ReadBytes(0x55);
            Unknown2 = ReadInt32(TestReader, EndianWSG);
            PromoCodesUsed = ReadListInt32(TestReader, EndianWSG);
            PromoCodesRequiringNotification = ReadListInt32(TestReader, EndianWSG);
            NumberOfEchoLists = ReadEchoes(TestReader, EndianWSG);

            DLC.DataSections = new List<DLCSection>();
            DLC.DLC_Size = ReadInt32(TestReader, EndianWSG);
            byte[] dlcDataBlock = TestReader.ReadBytes(DLC.DLC_Size);
            if (dlcDataBlock.Length != DLC.DLC_Size)
                throw new EndOfStreamException();

            using (BinaryReader dlcDataReader = new BinaryReader(new MemoryStream(dlcDataBlock, false), Encoding.ASCII))
            {
                int RemainingBytes = DLC.DLC_Size;
                while (RemainingBytes > 0)
                {
                    DLCSection Section = new DLCSection();
                    Section.Id = ReadInt32(dlcDataReader, EndianWSG);
                    int SectionLength = ReadInt32(dlcDataReader, EndianWSG);
                    long SectionStartPos = (int)dlcDataReader.BaseStream.Position;
                    switch (Section.Id)
                    {
                        case DLC_Data.Section1Id: // 0x43211234
                            DLC.HasSection1 = true;
                            DLC.DLC_Unknown1 = dlcDataReader.ReadByte();
                            DLC.BankSize = ReadInt32(dlcDataReader, EndianWSG);
                            int bankEntriesCount = ReadInt32(dlcDataReader, EndianWSG);
                            DLC.BankInventory = new List<BankEntry>();
                            for (int i = 0; i < bankEntriesCount; i++)
                                DLC.BankInventory.Add(ReadBankEntry(dlcDataReader, EndianWSG, SaveValuesCount));
                            break;
                        case DLC_Data.Section2Id: // 0x02151984
                            DLC.HasSection2 = true;
                            DLC.DLC_Unknown2 = ReadInt32(dlcDataReader, EndianWSG);
                            DLC.DLC_Unknown3 = ReadInt32(dlcDataReader, EndianWSG);
                            DLC.DLC_Unknown4 = ReadInt32(dlcDataReader, EndianWSG);
                            DLC.SkipDLC2Intro = ReadInt32(dlcDataReader, EndianWSG);
                            break;
                        case DLC_Data.Section3Id: // 0x32235947
                            DLC.HasSection3 = true;
                            DLC.DLC_Unknown5 = dlcDataReader.ReadByte();
                            break;
                        case DLC_Data.Section4Id: // 0x234ba901
                            DLC.HasSection4 = true;
                            DLC.SecondaryPackEnabled = dlcDataReader.ReadByte();
 
                            try
                            {
                                DLC.NumberOfItems = ReadItems(dlcDataReader, EndianWSG);
                            }
                            catch
                            {
                                // The data was invalid so the processing ran into an exception.
                                // See if the user wants to ignore the invalid data and just try
                                // to recover partial data.  If not, just re-throw the exception.
                                if (AutoRepair == false)
                                    throw;

                                // Set the flag to indicate that repair was required to load the savegame
                                RequiredRepair = true;

                                // Make sure there's no half-processed item that only added
                                // to ItemStrings but crashed before adding ItemValues.
                                // Remove the excess if there is.
                                if (ItemStrings.Count > ItemValues.Count)
                                    ItemStrings.RemoveAt(ItemStrings.Count - 1);

                                // Figure out how many weapons were successfully read
                                DLC.NumberOfWeapons = ItemStrings.Count - NumberOfItems;
                                
                                // If the data is invalid here, the whole DLC weapon list is invalid so
                                // set its length to 0 and be done
                                DLC.NumberOfWeapons = 0;

                                // Skip to the end of the section to discard any raw data that is left over
                                dlcDataReader.BaseStream.Position = SectionStartPos + SectionLength;
                            }
                            NumberOfItems += DLC.NumberOfItems;

                            try
                            {
                                DLC.NumberOfWeapons = ReadWeapons(dlcDataReader, EndianWSG);
                            }
                            catch
                            {
                                // The data was invalid so the processing ran into an exception.
                                // See if the user wants to ignore the invalid data and just try
                                // to recover partial data.  If not, just re-throw the exception.
                                if (AutoRepair == false)
                                    throw;

                                // Set the flag to indicate that repair was required to load the savegame
                                RequiredRepair = true;

                                // Make sure there's no half-processed weapon that only added
                                // to WeaponStrings but crashed before adding WeaponValues.
                                // Remove the excess if there is.
                                if (WeaponStrings.Count > WeaponValues.Count)
                                    WeaponStrings.RemoveAt(WeaponStrings.Count - 1);

                                // Figure out how many weapons were successfully read
                                DLC.NumberOfWeapons = WeaponStrings.Count - NumberOfWeapons;

                                // Skip to the end of the section to discard any raw data that is left over
                                dlcDataReader.BaseStream.Position = SectionStartPos + SectionLength;
                            }
                            NumberOfWeapons += DLC.NumberOfWeapons;
                            break;
                        default:
                            break;
                    }

                    // I don't pretend to know if any of the DLC sections will ever expand
                    // and store more data.  RawData stores any extra data at the end of
                    // the known data in any section and stores the entirety of sections 
                    // with unknown ids in a buffer in its raw byte order dependent form.
                    int RawDataCount = SectionLength - (int)(dlcDataReader.BaseStream.Position - SectionStartPos);

                    Section.RawData = dlcDataReader.ReadBytes(RawDataCount);
                    if (RawDataCount > 0)
                        ContainsRawData = true;
                    RemainingBytes -= SectionLength + 8;
                    DLC.DataSections.Add(Section);
                }
            }
            if (RevisionNumber >= 0x27)
                EnhancedBlock2 = TestReader.ReadBytes(0x50);
        }

        private int ReadEchoes(BinaryReader reader, ByteOrder EndianWSG)
        {
            int echoListCount = ReadInt32(reader, EndianWSG);

            EchoLists.Clear();
            for (int i = 0; i < echoListCount; i++)
            {
                EchoTable et = new EchoTable();
                et.Index = ReadInt32(reader, EndianWSG);
                et.TotalEchoes = ReadInt32(reader, EndianWSG);
                et.Echoes = new List<EchoEntry>();

                for (int echoIndex = 0; echoIndex < et.TotalEchoes; echoIndex++)
                {
                    EchoEntry ee = new EchoEntry();
                    ee.Name = ReadString(reader, EndianWSG);
                    ee.DLCValue1 = ReadInt32(reader, EndianWSG);
                    ee.DLCValue2 = ReadInt32(reader, EndianWSG);
                    et.Echoes.Add(ee);
                }
                EchoLists.Add(et);
            }
            return echoListCount;
        }
        
        private int ReadQuests(BinaryReader reader, ByteOrder EndianWSG)
        {
            int NumberOfQuestLists = ReadInt32(reader, EndianWSG);

            QuestLists.Clear();
            for (int listIndex = 0; listIndex < NumberOfQuestLists; listIndex++)
            {
                QuestTable qt = new QuestTable();
                qt.Index = ReadInt32(reader, EndianWSG);
                qt.CurrentQuest = ReadString(reader, EndianWSG);
                qt.TotalQuests = ReadInt32(reader, EndianWSG);
                qt.Quests = new List<QuestEntry>();
                int questCount = qt.TotalQuests;

                for (int questIndex = 0; questIndex < questCount; questIndex++)
                {
                    QuestEntry qe = new QuestEntry();
                    qe.Name = ReadString(reader, EndianWSG);
                    qe.Progress = ReadInt32(reader, EndianWSG);
                    qe.DLCValue1 = ReadInt32(reader, EndianWSG);
                    qe.DLCValue2 = ReadInt32(reader, EndianWSG);

                    int objectiveCount = ReadInt32(reader, EndianWSG);
                    qe.NumberOfObjectives = objectiveCount;
                    qe.Objectives = new QuestObjective[objectiveCount];

                    for (int objectiveIndex = 0; objectiveIndex < objectiveCount; objectiveIndex++)
                    {
                        qe.Objectives[objectiveIndex].Description = ReadString(reader, EndianWSG);
                        qe.Objectives[objectiveIndex].Progress = ReadInt32(reader, EndianWSG);
                    }
                    qt.Quests.Add(qe);
                }

                if (qt.CurrentQuest == "None" & qt.Quests.Count > 0) 
                    qt.CurrentQuest = qt.Quests[0].Name;

                QuestLists.Add(qt);
            }
            return NumberOfQuestLists;
        }

        private int ReadSkills(BinaryReader reader, ByteOrder EndianWSG)
        {
            int skillsCount = ReadInt32(reader, EndianWSG);

            string[] TempSkillNames = new string[skillsCount];
            int[] TempLevelOfSkills = new int[skillsCount];
            int[] TempExpOfSkills = new int[skillsCount];
            int[] TempInUse = new int[skillsCount];

            for (int Progress = 0; Progress < skillsCount; Progress++)
            {
                TempSkillNames[Progress] = ReadString(reader, EndianWSG);
                TempLevelOfSkills[Progress] = ReadInt32(reader, EndianWSG);
                TempExpOfSkills[Progress] = ReadInt32(reader, EndianWSG);
                TempInUse[Progress] = ReadInt32(reader, EndianWSG);
            }

            SkillNames = TempSkillNames;
            LevelOfSkills = TempLevelOfSkills;
            ExpOfSkills = TempExpOfSkills;
            InUse = TempInUse;

            return skillsCount;
        }

        private int ReadAmmo(BinaryReader reader, ByteOrder EndianWSG)
        {
            int poolsCount = ReadInt32(reader, EndianWSG);
 
            string[] TempResourcePools = new string[poolsCount];
            string[] TempAmmoPools = new string[poolsCount];
            float[] TempRemainingPools = new float[poolsCount];
            int[] TempPoolLevels = new int[poolsCount];

            for (int Progress = 0; Progress < poolsCount; Progress++)
            {
                TempResourcePools[Progress] = ReadString(reader, EndianWSG);
                TempAmmoPools[Progress] = ReadString(reader, EndianWSG);
                TempRemainingPools[Progress] = ReadSingle(reader, EndianWSG);
                TempPoolLevels[Progress] = ReadInt32(reader, EndianWSG);
            }

            ResourcePools = TempResourcePools;
            AmmoPools = TempAmmoPools;
            RemainingPools = TempRemainingPools;
            PoolLevels = TempPoolLevels;

            return poolsCount;
        }

        private int ReadItems(BinaryReader reader, ByteOrder EndianWSG)
        {
            int itemCount = ReadInt32(reader, EndianWSG);

            for (int Progress = 0; Progress < itemCount; Progress++)
            {
                List<string> strings = new List<string>();
                for (int TotalStrings = 0; TotalStrings < 9; TotalStrings++)
                    strings.Add(ReadString(reader, EndianWSG));
                ItemStrings.Add(strings);

                List<int> values = new List<int>();
                Int32 Value1 = ReadInt32(reader, EndianWSG);
                UInt32 tempLevelQuality = (UInt32)ReadInt32(reader, EndianWSG);
                Int16 Quality = (Int16)(tempLevelQuality % (UInt32)65536);
                Int16 Level = (Int16)(tempLevelQuality / (UInt32)65536);
                Int32 EquippedSlot = ReadInt32(reader, EndianWSG);
                values.Add(Value1);
                values.Add(Quality);
                values.Add(EquippedSlot);
                values.Add(Level);
                if (SaveValuesCount > 4)
                {
                    Int32 IsJunk = ReadInt32(reader, EndianWSG);
                    Int32 IsLocked = ReadInt32(reader, EndianWSG);
                    values.Add(IsJunk);
                    values.Add(IsLocked);
                }
                ItemValues.Add(values);
            }
            return itemCount;
        }

        private int ReadWeapons(BinaryReader reader, ByteOrder EndianWSG)
        {
            int weaponCount = ReadInt32(reader, EndianWSG);

            for (int Progress = 0; Progress < weaponCount; Progress++)
            {
                List<string> strings = new List<string>();

                for (int TotalStrings = 0; TotalStrings < 14; TotalStrings++)
                    strings.Add(ReadString(reader, EndianWSG));

                Int32 AmmoCount = ReadInt32(reader, EndianWSG);
                UInt32 tempLevelQuality = (UInt32)ReadInt32(reader, EndianWSG);
                Int16 Level = (Int16)(tempLevelQuality / (UInt32)65536);
                Int16 Quality = (Int16)(tempLevelQuality % (UInt32)65536);
                Int32 EquippedSlot = ReadInt32(reader, EndianWSG);

                List<int> values = new List<int>() {
                    AmmoCount,
                    Quality,
                    EquippedSlot,
                    Level
                };

                WeaponStrings.Add(strings);

                if (SaveValuesCount > 4)
                {
                    Int32 isJunk = ReadInt32(reader, EndianWSG);
                    Int32 isLocked = ReadInt32(reader, EndianWSG);
                    values.Add(isJunk);
                    values.Add(isLocked);
                }
                WeaponValues.Add(values);
            }
            return weaponCount;
        }

        private int ReadChallenges(BinaryReader reader, ByteOrder EndianWSG)
        {
            ChallengeDataBlockLength = ReadInt32(reader, EndianWSG);
            byte[] challengeDataBlock = reader.ReadBytes(ChallengeDataBlockLength);
            if (challengeDataBlock.Length != ChallengeDataBlockLength)
                throw new EndOfStreamException();

            using (BinaryReader challengeReader = new BinaryReader(new MemoryStream(challengeDataBlock, false), Encoding.ASCII))
            {
                ChallengeDataBlockId = ReadInt32(challengeReader, EndianWSG);
                ChallengeDataLength = ReadInt32(challengeReader, EndianWSG);
                ChallengeDataEntries = ReadInt16(challengeReader, EndianWSG);
                Challenges = new List<ChallengeDataEntry>();
                List<string> sections = db.ChallengesXml.stListSectionNames();
                for (int i = 0; i < ChallengeDataEntries; i++)
                {
                    ChallengeDataEntry challenge;
                    challenge.Id = challengeReader.ReadByte();
                    challenge.Static = ReadInt16(challengeReader, EndianWSG);
                    challenge.Value = ReadInt32(challengeReader, EndianWSG);

                    string name, description, level1,value1,level2,value2,level3,value3,level4,value4;

                    string section = challenge.Id.ToString();
                    if ( sections.Contains(section))
                    {
                        name = db.ChallengesXml.XmlReadValue(section, "Group");
                        description = db.ChallengesXml.XmlReadValue(section, "Description");
                        level1 = db.ChallengesXml.XmlReadValue(section, "Level1");
                        value1 = db.ChallengesXml.XmlReadValue(section, "Value1");
                        level2 = db.ChallengesXml.XmlReadValue(section, "Level2");
                        value2 = db.ChallengesXml.XmlReadValue(section, "Value2");
                        level3 = db.ChallengesXml.XmlReadValue(section, "Level3");
                        value3 = db.ChallengesXml.XmlReadValue(section, "Value3");
                        level4 = db.ChallengesXml.XmlReadValue(section, "Level4");
                        value4 = db.ChallengesXml.XmlReadValue(section, "Value4");
                    }
                    else
                    {
                        name = "";
                        description = "";
                        level1 = "";
                        value1 = "";
                        level2 = "";
                        value2 = "";
                        level3 = "";
                        value3 = "";
                        level4 = "";
                        value4 = "";
                    }

                    challenge.Name = name;
                    challenge.Description = description;
                    challenge.Level1 = level1;
                    challenge.Value1 = value1;
                    challenge.Level2 = level2;
                    challenge.Value2 = value2;
                    challenge.Level3 = level3;
                    challenge.Value3 = value3;
                    challenge.Level4 = level4;
                    challenge.Value4 = value4;

                    Challenges.Add(challenge);
                }
            }
            return 0;
        }

        private int ReadLocations(BinaryReader reader, ByteOrder EndianWSG)
        {
            int locationCount = ReadInt32(reader, EndianWSG);
            string[] tempLocationStrings = new string[locationCount];

            for (int Progress = 0; Progress < locationCount; Progress++)
                tempLocationStrings[Progress] = ReadString(reader, EndianWSG);

            LocationStrings = tempLocationStrings;
            return locationCount;
        }

        public void DiscardRawData()
        {
            // Make a list of all the known data sections to compare against.
            List<int> KnownSectionIds = new List<int>() {
                DLC_Data.Section1Id,
                DLC_Data.Section2Id,
                DLC_Data.Section3Id,
                DLC_Data.Section4Id,
            };

            // Traverse the list of data sections from end to beginning because when
            // an item gets deleted it does not affect the index of the ones before it,
            // but it does change the index of the ones after it.
            for (int i = this.DLC.DataSections.Count - 1; i >= 0; i--)
            {
                WillowSaveGame.DLCSection section = this.DLC.DataSections[i];

                if (KnownSectionIds.Contains(section.Id))
                {
                    // clear the raw data in this DLC data section
                    section.RawData = new byte[0];
                }
                else
                {
                    // if the section id is not recognized remove it completely
                    section.RawData = null;
                    this.DLC.DataSections.RemoveAt(i);
                }
            }

            // Now that all the raw data has been removed, reset the raw data flag
            this.ContainsRawData = false;
        }

        ///<summary>Save the current data to a WSG as a byte[]</summary>
        public byte[] WriteWSG()
        {
            MemoryStream OutStream = new MemoryStream();
            BinaryWriter Out = new BinaryWriter(OutStream);

            SplitInventoryIntoPacks();

            Out.Write(Encoding.ASCII.GetBytes(MagicHeader));
            Write(Out, VersionNumber, EndianWSG);
            Out.Write(Encoding.ASCII.GetBytes(PLYR));
            Write(Out, RevisionNumber, EndianWSG);
            Write(Out, Class, EndianWSG);
            Write(Out, Level, EndianWSG);
            Write(Out, Experience, EndianWSG);
            Write(Out, SkillPoints, EndianWSG);
            Write(Out, Unknown1, EndianWSG);
            Write(Out, Cash, EndianWSG);
            Write(Out, FinishedPlaythrough1, EndianWSG);
            Write(Out, NumberOfSkills, EndianWSG);

            for (int Progress = 0; Progress < NumberOfSkills; Progress++) //Write Skills
            {
                Write(Out, SkillNames[Progress], EndianWSG);
                Write(Out, LevelOfSkills[Progress], EndianWSG);
                Write(Out, ExpOfSkills[Progress], EndianWSG);
                Write(Out, InUse[Progress], EndianWSG);
            }

            Write(Out, Vehi1Color, EndianWSG);
            Write(Out, Vehi2Color, EndianWSG);
            Write(Out, Vehi1Type, EndianWSG);
            Write(Out, Vehi2Type, EndianWSG);
            Write(Out, NumberOfPools, EndianWSG);

            for (int Progress = 0; Progress < NumberOfPools; Progress++) //Write Ammo Pools
            {
                Write(Out, ResourcePools[Progress], EndianWSG);
                Write(Out, AmmoPools[Progress], EndianWSG);
                Write(Out, RemainingPools[Progress], EndianWSG);
                Write(Out, PoolLevels[Progress], EndianWSG);
            }

            Write(Out, ItemStrings1.Count, EndianWSG);
            for (int Progress = 0; Progress < ItemStrings1.Count; Progress++) //Write Items
            {
                for (int TotalStrings = 0; TotalStrings < 9; TotalStrings++)
                    Write(Out, ItemStrings1[Progress][TotalStrings], EndianWSG);

                Write(Out, ItemValues1[Progress][0], EndianWSG);
                UInt32 tempLevelQuality = (UInt16)ItemValues1[Progress][1] + (UInt16)ItemValues1[Progress][3] * (UInt32)65536;
                Write(Out, (Int32)tempLevelQuality, EndianWSG);
                Write(Out, ItemValues1[Progress][2], EndianWSG);
                if (RevisionNumber >= 0x27)
                {
                    Write(Out, ItemValues1[Progress][4], EndianWSG);
                    Write(Out, ItemValues1[Progress][5], EndianWSG);
                };
            }

            Write(Out, BackpackSize, EndianWSG);
            Write(Out, EquipSlots, EndianWSG);

            Write(Out, WeaponStrings1.Count, EndianWSG);
            for (int Progress = 0; Progress < WeaponStrings1.Count; Progress++) //Write Weapons
            {
                for (int TotalStrings1 = 0; TotalStrings1 < 14; TotalStrings1++)
                    Write(Out, WeaponStrings1[Progress][TotalStrings1], EndianWSG);

                Write(Out, WeaponValues1[Progress][0], EndianWSG);
                UInt32 tempLevelQuality = (UInt16)WeaponValues1[Progress][1] + (UInt16)WeaponValues1[Progress][3] * (UInt32)65536;
                Write(Out, (Int32)tempLevelQuality, EndianWSG);
                Write(Out, WeaponValues1[Progress][2], EndianWSG);
                if (RevisionNumber >= 0x27)
                {
                    Write(Out, WeaponValues1[Progress][4], EndianWSG);
                    Write(Out, WeaponValues1[Progress][5], EndianWSG);
                };
            }

            Int16 count = (Int16)Challenges.Count();
            Write(Out, count * 7 + 10, EndianWSG);
            Write(Out, ChallengeDataBlockId, EndianWSG);
            Write(Out, count * 7 + 2, EndianWSG);
            Write(Out, count, EndianWSG);
            foreach (ChallengeDataEntry challenge in Challenges)
            {
                Out.Write(challenge.Id);
                Write(Out, challenge.Static, EndianWSG);
                Write(Out, challenge.Value, EndianWSG);
            }

            Write(Out, TotalLocations, EndianWSG);

            for (int Progress = 0; Progress < TotalLocations; Progress++) //Write Locations
                Write(Out, LocationStrings[Progress], EndianWSG);

            Write(Out, CurrentLocation, EndianWSG);
            Write(Out, SaveInfo1to5[0], EndianWSG);
            Write(Out, SaveInfo1to5[1], EndianWSG);
            Write(Out, SaveInfo1to5[2], EndianWSG);
            Write(Out, SaveInfo1to5[3], EndianWSG);
            Write(Out, SaveInfo1to5[4], EndianWSG);
            Write(Out, SaveNumber, EndianWSG);
            Write(Out, SaveInfo7to10[0], EndianWSG);
            Write(Out, SaveInfo7to10[1], EndianWSG);
            Write(Out, NumberOfQuestLists, EndianWSG);

            for (int listIndex = 0; listIndex < NumberOfQuestLists; listIndex++)
            {
                QuestTable qt = QuestLists[listIndex];
                Write(Out, qt.Index, EndianWSG);
                Write(Out, qt.CurrentQuest, EndianWSG);
                Write(Out, qt.TotalQuests, EndianWSG);

                int questCount = qt.TotalQuests;
                for (int questIndex = 0; questIndex < questCount; questIndex++)  //Write Playthrough 1 Quests
                {
                    QuestEntry qe = qt.Quests[questIndex];
                    Write(Out, qe.Name, EndianWSG);
                    Write(Out, qe.Progress, EndianWSG);
                    Write(Out, qe.DLCValue1, EndianWSG);
                    Write(Out, qe.DLCValue2, EndianWSG);

                    int objectiveCount = qe.NumberOfObjectives;
                    Write(Out, objectiveCount, EndianWSG);

                    for (int i = 0; i < objectiveCount; i++)
                    {
                        Write(Out, qe.Objectives[i].Description, EndianWSG);
                        Write(Out, qe.Objectives[i].Progress, EndianWSG);
                    }
                }
            }

            Write(Out, TotalPlayTime, EndianWSG);
            Write(Out, LastPlayedDate, EndianWSG);
            Write(Out, CharacterName, EndianWSG);
            Write(Out, Color1, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Write(Out, Color2, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)
            Write(Out, Color3, EndianWSG); //ABGR Big (X360, PS3), RGBA Little (PC)

            if (RevisionNumber >= 0x27)
            {
                Out.Write(EnhancedBlock);
            }
            Write(Out, Unknown2, EndianWSG);

            int NumberOfPromoCodesUsed = PromoCodesUsed.Count;
            Write(Out, NumberOfPromoCodesUsed, EndianWSG);
            for (int i = 0; i < NumberOfPromoCodesUsed; i++)
                Write(Out, PromoCodesUsed[i], EndianWSG);

            int NumberOfPromoCodesRequiringNotification = PromoCodesRequiringNotification.Count;
            Write(Out, NumberOfPromoCodesRequiringNotification, EndianWSG);
            for (int i = 0; i < NumberOfPromoCodesRequiringNotification; i++)
                Write(Out, PromoCodesRequiringNotification[i], EndianWSG);
 
            Write(Out, NumberOfEchoLists, EndianWSG);
            for (int listIndex = 0; listIndex < NumberOfEchoLists; listIndex++)
            {
                EchoTable et = EchoLists[listIndex];
                Write(Out, et.Index, EndianWSG);
                Write(Out, et.TotalEchoes, EndianWSG);

                for (int echoIndex = 0; echoIndex < et.TotalEchoes; echoIndex++) //Write Locations
                {
                    EchoEntry ee = et.Echoes[echoIndex];
                    Write(Out, ee.Name, EndianWSG);
                    Write(Out, ee.DLCValue1, EndianWSG);
                    Write(Out, ee.DLCValue2, EndianWSG);
                }
            }

            DLC.DLC_Size = 0;
            // This loop writes the base data for each section into byte[]
            // BaseData so its size can be obtained and it can easily be
            // written to the output stream as a single block.  Calculate
            // DLC.DLC_Size as it goes since that has to be written before
            // the blocks are written to the output stream.
            foreach (DLCSection Section in DLC.DataSections)
            {
                MemoryStream tempStream = new MemoryStream();
                BinaryWriter memwriter = new BinaryWriter(tempStream);
                switch (Section.Id)
                {
                    case DLC_Data.Section1Id:
                        memwriter.Write(DLC.DLC_Unknown1);
                        Write(memwriter, DLC.BankSize, EndianWSG);
                        Write(memwriter, DLC.BankInventory.Count, EndianWSG);
                        for (int i = 0; i < DLC.BankInventory.Count; i++)
                            WriteBankEntry(memwriter, DLC.BankInventory[i], EndianWSG, SaveValuesCount);
                        break;
                    case DLC_Data.Section2Id:
                        Write(memwriter, DLC.DLC_Unknown2, EndianWSG);
                        Write(memwriter, DLC.DLC_Unknown3, EndianWSG);
                        Write(memwriter, DLC.DLC_Unknown4, EndianWSG);
                        Write(memwriter, DLC.SkipDLC2Intro, EndianWSG);
                        break;
                    case DLC_Data.Section3Id:
                        memwriter.Write(DLC.DLC_Unknown5);
                        break;
                    case DLC_Data.Section4Id:
                        memwriter.Write(DLC.SecondaryPackEnabled);
                        // The DLC backpack items
                        Write(memwriter, ItemStrings2.Count, EndianWSG);
                        for (int Progress = 0; Progress < ItemStrings2.Count; Progress++) //Write Items
                        {
                            for (int TotalStrings = 0; TotalStrings < 9; TotalStrings++)
                                Write(memwriter, ItemStrings2[Progress][TotalStrings], EndianWSG);

                            Write(memwriter, ItemValues2[Progress][0], EndianWSG);
                            UInt32 tempLevelQuality = (UInt16)ItemValues2[Progress][1] + (UInt16)ItemValues2[Progress][3] * (UInt32)65536;
                            Write(memwriter, (Int32)tempLevelQuality, EndianWSG);
                            Write(memwriter, ItemValues2[Progress][2], EndianWSG);
                            if (RevisionNumber >= 0x27)
                            {
                                Write(memwriter, ItemValues2[Progress][4], EndianWSG);
                                Write(memwriter, ItemValues2[Progress][5], EndianWSG);
                            };

                        }
                        // The DLC backpack weapons
                        Write(memwriter, WeaponStrings2.Count, EndianWSG);
                        for (int Progress = 0; Progress < WeaponStrings2.Count; Progress++) //Write DLC.Weapons
                        {
                            for (int TotalStrings = 0; TotalStrings < 14; TotalStrings++)
                                Write(memwriter, WeaponStrings2[Progress][TotalStrings], EndianWSG);

                            Write(memwriter, WeaponValues2[Progress][0], EndianWSG);
                            UInt32 tempLevelQuality = (UInt16)WeaponValues2[Progress][1] + (UInt16)WeaponValues2[Progress][3] * (UInt32)65536;
                            Write(memwriter, (Int32)tempLevelQuality, EndianWSG);
                            Write(memwriter, WeaponValues2[Progress][2], EndianWSG);
                            if (RevisionNumber >= 0x27)
                            {
                                Write(memwriter, WeaponValues2[Progress][4], EndianWSG);
                                Write(memwriter, WeaponValues2[Progress][5], EndianWSG);
                            };
                        }
                        break;
                    default:
                        break;
                }
                Section.BaseData = tempStream.ToArray();
                DLC.DLC_Size += Section.BaseData.Count() + Section.RawData.Count() + 8; // 8 = 4 bytes for id, 4 bytes for length
            }

            // Now its time to actually write all the data sections to the output stream
            Write(Out, DLC.DLC_Size, EndianWSG);
            foreach (DLCSection Section in DLC.DataSections)
            {
                Write(Out, Section.Id, EndianWSG);
                int SectionLength = Section.BaseData.Count() + Section.RawData.Count();
                Write(Out, SectionLength, EndianWSG);
                Out.Write(Section.BaseData);
                Out.Write(Section.RawData);
                Section.BaseData = null; // BaseData isn't needed anymore.  Free it.
            }

            if (RevisionNumber >= 0x27)
            {
                Out.Write(EnhancedBlock2);
            }

            // Clear the temporary lists used to split primary and DLC pack data
            ItemValues1 = null;
            ItemStrings1 = null;
            ItemValues2 = null;
            ItemValues2 = null;
            WeaponValues1 = null;
            WeaponStrings1 = null;
            WeaponValues2 = null;
            WeaponValues2 = null;
            return OutStream.ToArray();
        }
        ///<summary>Split the weapon and item lists into two parts: one for the primary pack and one for DLC backpack</summary>
        public void SplitInventoryIntoPacks()
        {
            // Split items and weapons into two lists each so they can be put into the 
            // DLC backpack or regular backpack area as needed.  Any item with a level 
            // override goes in the DLC backpack.  All others go in the regular inventory.
            if ((DLC.HasSection4 == false) || (DLC.SecondaryPackEnabled == 0))
            {
                // no secondary pack so put it all in primary pack
                ItemStrings1 = ItemStrings;
                ItemValues1 = ItemValues;
                WeaponStrings1 = WeaponStrings;
                WeaponValues1 = WeaponValues;
                ItemStrings2 = new List<List<string>>();
                ItemValues2 = new List<List<int>>();
                WeaponStrings2 = new List<List<string>>();
                WeaponValues2 = new List<List<int>>();
                return;
            }

            ItemStrings1 = new List<List<string>>();
            ItemValues1 = new List<List<int>>();
            ItemStrings2 = new List<List<string>>();
            ItemValues2 = new List<List<int>>();

            int ItemCount = ItemStrings.Count;
            for (int i = 0; i < ItemCount; i++)
            {
                if ((ItemValues[i][3] == 0) && (ItemStrings[i][0].Substring(0, 3) != "dlc"))
                {
                    ItemStrings1.Add(ItemStrings[i]);
                    ItemValues1.Add(ItemValues[i]);
                }
                else
                {
                    ItemStrings2.Add(ItemStrings[i]);
                    ItemValues2.Add(ItemValues[i]);
                }
            }

            WeaponStrings1 = new List<List<string>>();
            WeaponValues1 = new List<List<int>>();
            WeaponStrings2 = new List<List<string>>();
            WeaponValues2 = new List<List<int>>();

            int WeaponCount = WeaponStrings.Count;
            for (int i = 0; i < WeaponCount; i++)
            {
                if ((WeaponValues[i][3] == 0))
                {
                    WeaponStrings1.Add(WeaponStrings[i]);
                    WeaponValues1.Add(WeaponValues[i]);
                }
                else
                {
                    WeaponStrings2.Add(WeaponStrings[i]);
                    WeaponValues2.Add(WeaponValues[i]);
                }
            }
        }

        public sealed class BankEntry
        {
            public Byte TypeId;
            public List<string> Parts = new List<string>();
            public Int32 Quantity; //AmmoOrQuantity
            public Byte Equipped;
            public Int16 Quality;
            public Int16 Level;
            public Byte IsJunk;
            public Byte IsLocked;
        }

        private static string ReadBankString(BinaryReader reader, ByteOrder EndianWSG)
        {
            byte subObjectMask = reader.ReadByte();
            //if ((subObjectMask != 32) && (subObjectMask != 0))
            //    throw new FileFormatException("Bank string has an unknown sub-object mask.  Mask = " + subObjectMask);

            string composed = ReadString(reader, EndianWSG);
            bool isPreviousSubObject = (subObjectMask & 1) == 1;
            subObjectMask >>= 1;

            for (int i = 1; i < 6; i++)
            {
                string substring = ReadString(reader, EndianWSG);
                if (!string.IsNullOrEmpty(substring))
                {
                    if (isPreviousSubObject)
                        if (string.IsNullOrEmpty(composed))
                            composed = substring;
                        else
                            composed += ":" + substring;
                    else
                        if (string.IsNullOrEmpty(composed))
                            composed = substring;
                        else
                            composed += "." + substring;
                }

                isPreviousSubObject = (subObjectMask & 1) == 1;
                subObjectMask >>= 1;
            }

            return composed;
        }

        private static void WriteBankString(BinaryWriter bw, string value, ByteOrder Endian)
        {
            if (string.IsNullOrEmpty(value))
            {
                // Endianness does not matter here.
                bw.Write((byte)0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
                bw.Write(0);
            }
            else
            {
                byte subObjectMask = 32;
                string[] pathComponentNames = value.Split('.');

                bw.Write(subObjectMask);

                // Write the empty strings first.
                // Endianness does not matter here.
                for (int j = pathComponentNames.Length; j < 6; j++)
                    bw.Write(0);

                // Then write the strings that are not empty.
                for (int j = 0; j < pathComponentNames.Length; j++)
                    Write(bw, pathComponentNames[j], Endian);
            }
        }

        private static BankEntry ReadBankEntry(BinaryReader reader, ByteOrder endian, int valuesCount)
        {
            BankEntry entry = new BankEntry();
            int partCount;

            entry.TypeId = reader.ReadByte();

            switch (entry.TypeId)
            {
                case 1: // weapon
                    // For some reason the manufacturer (parts[2]) and weapon type definition (parts[1])
                    // are exchanged in the bank compared to other inventory lists in the savegame.
                    // Account for that here by reading them in the order they are stored 
                    // in the save, but adding them to the part list in the standard order used in the weapon
                    // and item tabs.
                    string[] parts = new string[3]
                    {
                        ReadBankString(reader, endian),
                        ReadBankString(reader, endian),
                        ReadBankString(reader, endian)
                    };

                    entry.Parts.Add(parts[1]);
                    entry.Parts.Add(parts[2]);
                    entry.Parts.Add(parts[0]);
                    break;
                case 2: // item
                    for (int i = 0; i < 3; i++)
                        entry.Parts.Add(ReadBankString(reader, endian));
                    break;
                default:
                    throw new FormatException("Bank entry to be written has invalid Type ID.  TypeId = " + entry.TypeId);
            }

            UInt32 temp = (UInt32)ReadInt32(reader, endian);
            entry.Quality = (Int16)(temp % (UInt32)65536);
            entry.Level = (Int16)(temp / (UInt32)65536);
            switch (entry.TypeId)
            {
                case 1:
                    partCount = 14;
                    break;
                case 2:
                    partCount = 9;
                    break;
                default:
                    partCount = 0;
                    break;
            }

            for (int i = 3; i < partCount; i++)
                entry.Parts.Add(ReadBankString(reader, endian));

            byte[] Footer = reader.ReadBytes(valuesCount > 4 ? 12 : 10);

            // da_fileserver's investigation has led him to believe the footer bytes are:
            // (Int)GameStage - default 0
            // (Int)AwesomeLevel - default 0
            // (Byte)Equipped - default 0
            // (Byte)DropOnDeath - default 1 (this is whether an npc would drop it when it dies not you)
            // matt911 - It seems apparent that this table is used for more than just the bank inventory 
            // in the game.  None of the values are stored in the inventory part of the savegame
            // except Equipped and even that will be updated immediately when you take the item
            // out of the bank.  I've never seen any of these with anything except the default value
            // in the bank except Equipped so I will store that in case it is not what we think it
            // is and it is important, but I am doubtful that it is.
            //
            // I put debug assertions that will only cause a breakpoint when running willowtree in a debug build.
            // They check each default value and warn me if any other values are found so I can
            // understand them better, but I have received no warnings yet after opening many dozens of savegames.
            for (int i = 0; i < 7; i++)
                System.Diagnostics.Debug.Assert(Footer[i] == 0);
            entry.Equipped = Footer[8];
            System.Diagnostics.Debug.Assert(Footer[9] == 1);

            switch (entry.TypeId)
            {
                case 1: // weapon
                    entry.Quantity = ReadInt32(reader, endian);

                    if (valuesCount > 4)
                    {
                        entry.IsJunk = Footer[10];
                        entry.IsLocked = Footer[11];
                    }
                    else
                    {
                        entry.IsJunk = 0;
                        entry.IsLocked = 0;
                    }
                    break;
                case 2: // item
                    if (valuesCount > 4)
                    {
                        entry.Quantity = 1;
                        entry.IsJunk = Footer[11];
                        entry.IsLocked = reader.ReadByte();
                        System.Diagnostics.Debug.Assert(Footer[10] == Footer[8]);
                    }
                    else
                    {
                        entry.Quantity = reader.ReadByte();
                        entry.IsJunk = 0;
                        entry.IsLocked = 0;
                    }

                    break;
                default:
                    entry.Quantity = 0;
                    break;
            }
            return entry;
        }
        private static void WriteBankEntry(BinaryWriter bw, BankEntry entry, ByteOrder Endian, int valuesCount)
        {
            if (entry.Parts.Count < 3)
                throw new FormatException("Bank entry has an invalid part count. Parts.Count = " + entry.Parts.Count);

            bw.Write(entry.TypeId);

            switch (entry.TypeId)
            {
                case 1:
                    // The parts are written in a different order in bank part of the savegame for some reason.
                    // Reorder them here to account for that.
                    WriteBankString(bw, entry.Parts[2], Endian);
                    WriteBankString(bw, entry.Parts[0], Endian);
                    WriteBankString(bw, entry.Parts[1], Endian);
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                        WriteBankString(bw, entry.Parts[i], Endian);
                    break;
                default:
                    throw new FormatException("Bank entry to be written has an invalid Type ID.  TypeId = " + entry.TypeId);
            }


            UInt32 grade = (UInt16)entry.Quality + (UInt16)entry.Level * (UInt32)65536;

            Write(bw, (Int32)grade, Endian);

            for (int i = 3; i < entry.Parts.Count; i++)
                WriteBankString(bw, entry.Parts[i], Endian);

            // see ReadBankEntry for notes about the footer bytes
            Byte[] Footer;
            if (valuesCount > 4)
                Footer = new Byte[12] { 0, 0, 0, 0, 0, 0, 0, 0, entry.Equipped, 1, entry.Equipped, entry.IsJunk };
            else
                Footer = new Byte[10] { 0, 0, 0, 0, 0, 0, 0, 0, entry.Equipped, 1 };

            bw.Write(Footer);

            switch (entry.TypeId)
            {
                case 1: // weapon
                    Write(bw, entry.Quantity, Endian);
                    break;
                case 2: // item
                    if (valuesCount > 4)
                        bw.Write((Byte)entry.IsLocked);
                    else
                        bw.Write((Byte)entry.Quantity);
                    break;
                default:
                    throw new FormatException("Bank entry to be written has an invalid Type ID.  TypeId = " + entry.TypeId);
            }
        }
    }
}
