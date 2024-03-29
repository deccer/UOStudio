using System.Collections.Generic;
using System.IO;

namespace UOStudio.Tools.TextureAtlasGenerator.Ultima
{
    public sealed class Animdata
    {
        private static int[] _header;
        private static byte[] _unknown;

        public static Dictionary<int, AnimdataEntry> AnimData { get; private set; }

        static Animdata()
        {
            Initialize();
        }

        /// <summary>
        /// Reads animdata.mul and fills <see cref="AnimData"/>
        /// </summary>
        public static void Initialize()
        {
            AnimData = new Dictionary<int, AnimdataEntry>();

            var path = Files.GetFilePath("animdata.mul");
            if (path == null)
            {
                return;
            }

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var bin = new BinaryReader(fs))
                {
                    unsafe
                    {
                        var id = 0;
                        var h = 0;

                        _header = new int[bin.BaseStream.Length / (4 + (8 * (64 + 4)))];

                        while (h < _header.Length)
                        {
                            _header[h++] = bin.ReadInt32(); // chunk header
                            // Read 8 tiles
                            var buffer = bin.ReadBytes(544);
                            fixed (byte* buf = buffer)
                            {
                                var data = buf;
                                for (var i = 0; i < 8; ++i, ++id)
                                {
                                    var fdata = new sbyte[64];
                                    for (var j = 0; j < 64; ++j)
                                    {
                                        fdata[j] = (sbyte)*data++;
                                    }

                                    var unk = *data++;
                                    var fcount = *data++;
                                    var finter = *data++;
                                    var fstart = *data++;
                                    if (fcount > 0)
                                    {
                                        AnimData[id] = new AnimdataEntry(fdata, unk, fcount, finter, fstart);
                                    }
                                }
                            }
                        }

                        var remaining = (int)(bin.BaseStream.Length - bin.BaseStream.Position);
                        if (remaining > 0)
                        {
                            _unknown = bin.ReadBytes(remaining);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets Animation <see cref="AnimdataEntry"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AnimdataEntry GetAnimData(int id)
        {
            return AnimData.ContainsKey(id) ? AnimData[id] : null;
        }

        public static void Save(string path)
        {
            var fileName = Path.Combine(path, "animdata.mul");
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var bin = new BinaryWriter(fs))
            {
                var id = 0;
                var h = 0;
                while (id < _header.Length * 8)
                {
                    bin.Write(_header[h++]);
                    for (var i = 0; i < 8; ++i, ++id)
                    {
                        var animdataEntry = GetAnimData(id);
                        for (var j = 0; j < 64; ++j)
                        {
                            if (animdataEntry != null)
                            {
                                bin.Write(animdataEntry.FrameData[j]);
                            }
                            else
                            {
                                bin.Write((sbyte)0);
                            }
                        }

                        if (animdataEntry != null)
                        {
                            bin.Write(animdataEntry.Unknown);
                            bin.Write(animdataEntry.FrameCount);
                            bin.Write(animdataEntry.FrameInterval);
                            bin.Write(animdataEntry.FrameStart);
                        }
                        else
                        {
                            bin.Write((byte)0);
                            bin.Write((byte)0);
                            bin.Write((byte)0);
                            bin.Write((byte)0);
                        }
                    }
                }

                if (_unknown != null)
                {
                    bin.Write(_unknown);
                }
            }
        }

        public class AnimdataEntry
        {
            public sbyte[] FrameData { get; set; }
            public byte Unknown { get; }
            public byte FrameCount { get; set; }
            public byte FrameInterval { get; set; }
            public byte FrameStart { get; set; }

            public AnimdataEntry(sbyte[] frame, byte unk, byte fcount, byte finter, byte fstart)
            {
                FrameData = frame;
                Unknown = unk;
                FrameCount = fcount;
                FrameInterval = finter;
                FrameStart = fstart;
            }
        }
    }
}
