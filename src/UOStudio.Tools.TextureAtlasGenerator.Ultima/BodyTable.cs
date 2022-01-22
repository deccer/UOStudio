using System;
using System.Collections.Generic;
using System.IO;

namespace UOStudio.TextureAtlasGenerator.Ultima
{
    public sealed class BodyTable
    {
        public static Dictionary<int, BodyTableEntry> Entries { get; private set; }

        static BodyTable()
        {
            Initialize();
        }

        public static void Initialize()
        {
            Entries = new Dictionary<int, BodyTableEntry>();

            var filePath = Files.GetFilePath("body.def");

            if (filePath == null)
            {
                return;
            }

            using (var def = new StreamReader(filePath))
            {
                string line;

                while ((line = def.ReadLine()) != null)
                {
                    if ((line = line.Trim()).Length == 0 || line.StartsWith("#"))
                    {
                        continue;
                    }

                    try
                    {
                        var index1 = line.IndexOf("{", StringComparison.Ordinal);
                        var index2 = line.IndexOf("}", StringComparison.Ordinal);

                        var param1 = line.Substring(0, index1);
                        var param2 = line.Substring(index1 + 1, index2 - index1 - 1);
                        var param3 = line.Substring(index2 + 1);

                        var indexOf = param2.IndexOf(',');

                        if (indexOf > -1)
                        {
                            param2 = param2.Substring(0, indexOf).Trim();
                        }

                        var iParam1 = Convert.ToInt32(param1.Trim());
                        var iParam2 = Convert.ToInt32(param2.Trim());
                        var iParam3 = Convert.ToInt32(param3.Trim());

                        Entries[iParam1] = new BodyTableEntry(iParam2, iParam1, iParam3);
                    }
                    catch
                    {
                        // TODO: ignored?
                        // ignored
                    }
                }
            }
        }
    }

    public sealed class BodyTableEntry
    {
        public int OldId { get; set; }
        public int NewId { get; set; }
        public int NewHue { get; set; }

        public BodyTableEntry(int oldId, int newId, int newHue)
        {
            OldId = oldId;
            NewId = newId;
            NewHue = newHue;
        }
    }
}