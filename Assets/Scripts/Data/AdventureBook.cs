using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ProjectFoundPhone.Data
{
    // The importer bakes portable images into texture sub-assets once. The player only reads them.
    public sealed class AdventureBook : ScriptableObject
    {
        public const string Format = "adventure-reader-v1";
        [SerializeField] private string bookTitle;
        [SerializeField, HideInInspector] private string pagesJson;
        [SerializeField] private Texture2D[] illustrations;
        [SerializeField] private AdventurePageStyle pageStyle;
        [SerializeField, HideInInspector] private string modelJson;

        public string Title => bookTitle;
        public IReadOnlyList<Texture2D> Illustrations => illustrations;
        public AdventurePageStyle Style => pageStyle;
        public string ModelJson => modelJson;
        public AdventurePage[] ReadPages() => JsonConvert.DeserializeObject<AdventurePage[]>(pagesJson);

        public void Initialize(string title, string pages, Texture2D[] images, AdventurePageStyle style = null, string model = null)
        {
            bookTitle = title;
            pagesJson = pages;
            illustrations = images;
            pageStyle = style;
            modelJson = model;
        }
    }

    [Serializable]
    public sealed class AdventurePageStyle
    {
        public string background, foreground, link;
        public bool sans, reducedMotion;
        public float width;
    }

    [Serializable]
    public sealed class AdventurePage
    {
        public string id;
        public string title;
        public AdventureBlock[] blocks;
    }

    [Serializable]
    public sealed class AdventureBlock
    {
        public string id;
        public string kind;
        public string text;
        public string role;
        public string target;
        public int image = -1;
        public List<AdventureBlock> children;

        public static AdventureBlock Text(string kind, string text) =>
            new AdventureBlock { kind = kind, text = text };

        public static AdventureBlock Group(string kind, string title = null) =>
            new AdventureBlock { kind = kind, text = title, children = new List<AdventureBlock>() };
    }
}
