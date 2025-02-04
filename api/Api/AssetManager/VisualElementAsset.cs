﻿using System.IO;
using System.Xml;
using SynthesisAPI.UIManager;
using SynthesisAPI.UIManager.VisualElements;
using SynthesisAPI.VirtualFileSystem;

namespace SynthesisAPI.AssetManager
{
    public class VisualElementAsset: Asset
    {
        private XmlDocument _document;

        public VisualElementAsset(string name, Permissions perms, string sourcePath)
        {
            Init(name, perms, sourcePath);
            _document = new XmlDocument();
        }

        public VisualElement GetElement(string name) => UIParser.CreateVisualElement(name, _document);

        public override IEntry Load(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            _document.Load(stream);
            return this;
        }
    }
}