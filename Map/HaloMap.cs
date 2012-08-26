using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quickbeam
{
    class HaloMap
    {
        private GCHandle? _haloMap = null;
        
        #region TagCollections
        // Perhaps these should provide some sort of error throwing if access
        // is attempted before loading a map?  That would require not using
        // the auto-implemented properties feature of C#...
        public HaloTag[] TagsInOrder
        {
            get; private set;
        }
        public ConcurrentDictionary<int, HaloTag> TagsByIdent
        {
            get; private set;
        }
        public ConcurrentDictionary<string, BlockingCollection<HaloTag>> TagsByClass
        {
            get; private set;
        }
        public IEnumerable<TreeViewItem> TreeViewTagClasses
        {
            get; private set;
        }
        #endregion TagCollections

        #region MapProperties
        public string Build { get; private set; }
        public string Integrity { get; private set; }
        public string Name { get; private set; }
        public int BaseTagId { get; private set; }
        public int IndexOffset { get; private set; }
        public int MapMagic { get; private set; }
        public int MemoryOffset { get; private set; }
        public int MetaDataLength { get; private set; }
        public int TagCount { get; private set; }
        public int Type { get; private set; }
        public int Version { get; private set; }
        #endregion MapProperties

        #region MapLoading

        internal void LoadMap2(string filename)
        {
            var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open);
            var accessor = mmf.CreateViewAccessor(0, 0); // Begin to end

            MapHeaderStruct mapHeader;
            accessor.Read(0, out mapHeader);
            IndexHeaderStruct indexHeader;
            accessor.Read(mapHeader.indexOffset, out indexHeader);


        }

        /// <summary>
        /// Loads all of the map's data into an internal object structure.
        /// </summary>
        /// <param name="filename">The path to the mapfile to load</param>
        internal HaloMap LoadMap(string filename)
        {
            // Grab the whole file
            if (_haloMap != null)
                _haloMap.Value.Free();
            _haloMap = GCHandle.Alloc(ReadFile(filename), GCHandleType.Pinned);

            // Read the map header
            MapHeaderStruct _mapHeader = (MapHeaderStruct)Marshal.PtrToStructure(
                _haloMap.Value.AddrOfPinnedObject(),
                typeof(MapHeaderStruct));
            Build = _mapHeader.mapBuild;
            IndexOffset = _mapHeader.indexOffset;
            Integrity = _mapHeader.integrity;
            MetaDataLength = _mapHeader.metaDataLength;
            Name = _mapHeader.mapName;
            Type = _mapHeader.mapType;
            Version = _mapHeader.gameVersion;

            // Read the index header
            var addrOfIndexHeader = _haloMap.Value.AddrOfPinnedObject() + IndexOffset;
            IndexHeaderStruct _indexHeader = (IndexHeaderStruct)Marshal.PtrToStructure(
                addrOfIndexHeader,
                typeof(IndexHeaderStruct));
            BaseTagId = _indexHeader.baseTagId;
            TagCount = _indexHeader.tagCount;
            MemoryOffset = _indexHeader.memoryOffset;

            // Calculate the map's magic
            MapMagic = MemoryOffset - IndexOffset - 40;

            // Read the tags
            readAllTags(addrOfIndexHeader, _haloMap.Value);

            return this;
        }

        private void readAllTags(System.IntPtr addrOfIndexHeader, GCHandle pinnedHaloMap)
        {
            // Prepare to read the tags
            TagsInOrder = new HaloTag[TagCount];
            TagsByIdent = new ConcurrentDictionary<int, HaloTag>();
            TagsByClass = new ConcurrentDictionary<string, BlockingCollection<HaloTag>>();
            
            var addrOfFirstTag = addrOfIndexHeader + Marshal.SizeOf(typeof(IndexHeaderStruct));
            var sizeOfHaloTagStruct = Marshal.SizeOf(typeof(HaloTagStruct));

            
            // In parallel, instantiate HaloTag objects and add them to the
            // appropriate collection.
            ParallelEnumerable.Range(0, TagCount)
                .ForAll(i =>
                {
                    // Read in a struct
                    HaloTagStruct hts = (HaloTagStruct)Marshal.PtrToStructure(
                        addrOfFirstTag + (i * sizeOfHaloTagStruct), // Calculate where the tag will be
                        typeof(HaloTagStruct));

                    // Convert to a nice object
                    HaloTag ht = new HaloTag(hts, MapMagic, pinnedHaloMap);

                    // Store in the proper collections
                    TagsInOrder[i] = ht;
                    TagsByIdent[ht.Ident] = ht;

                    // TagsByClass takes a bit more work.  For now, just make an empty list for each class.
                    // If we try to add the tags now, we'll have a race condition.
                    BlockingCollection<HaloTag> lookupResult;
                    var emptyClassList = TagsByClass.TryGetValue(ht.FirstClass, out lookupResult)
                        ? lookupResult
                        : new BlockingCollection<HaloTag>();
                    TagsByClass[ht.FirstClass] = emptyClassList;
                });

            #region ActionDefinitions
            Action finishTagsByClass = () =>
                {
                    // In parallel, finish TagsByClass now that the race condition is no longer an issue
                    // (since we're not having to create and store back empty lists)
                    ParallelEnumerable.Range(0, TagCount)
                        .ForAll(i =>
                        {
                            TagsByClass[TagsInOrder[i].FirstClass]
                                .Add(TagsInOrder[i]);
                        });
                };

            Action saveMetaLocations = () =>
                {
                    // A tag's meta ends at the start location of the following tag's meta
                    foreach (int i in Enumerable.Range(0, TagCount - 1))
                    {
                        TagsInOrder[i].NextTagMetaOffset = TagsInOrder[i + 1].MetaOffset;
                    }
                    // The length of the entire metadata area will let us know where the last meta ends
                    TagsInOrder[TagCount - 1].NextTagMetaOffset = MetaDataLength;
                };

            Thread addToTagTree = new Thread(new ThreadStart(() =>
            {
                List<TreeViewItem> toDisplay = new List<TreeViewItem>();

                List<string> tagClasses = TagsByClass.Keys.ToList();
                tagClasses.Sort();
                foreach (string tagClass in tagClasses)
                {
                    TreeViewItem classNode = new TreeViewItem();
                    classNode.Header = tagClass;
                }
            }));
            addToTagTree.SetApartmentState(ApartmentState.STA);

            Action parseMeta = () =>
                {
                    // Now that all the meta locations are stored, go and look for reflexives and such!
                    ParallelEnumerable.Range(0, TagCount)
                        .ForAll(i =>
                        {
                            TagsInOrder[i].readMeta(TagsByIdent.Keys, this);
                        });
                };
            #endregion ActionDefinitions

            // Be concurrent when possible
            Parallel.Invoke(finishTagsByClass, saveMetaLocations);
            addToTagTree.Start();
            parseMeta();
            
        }

        private static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;    // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
        #endregion MapLoading
    }
}
