using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectFoundPhone.Data;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    [ScriptedImporter(2, "adventure")]
    public sealed class AdventureBookImporter : ScriptedImporter
    {
        [MenuItem("Tools/Adventure/Prepare WritingPage HTML...")]
        private static void Prepare()
        {
            var file = Path.GetFullPath(Path.Combine(Application.dataPath, "../tools/prepare-adventure.html"));
            Application.OpenURL(new Uri(file).AbsoluteUri);
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var source = JObject.Parse(File.ReadAllText(ctx.assetPath));
            var pages = source["pages"] as JArray;
            var images = source["images"] as JArray;
            if ((string)source["format"] != AdventureBook.Format || pages == null || pages.Count == 0 || images == null)
                throw new InvalidDataException("WritingPage の変換ツールから保存した .adventure ファイルを選んでください。");

            var decoded = pages.ToObject<AdventurePage[]>();
            string model = source["model"]?.ToString(Formatting.None);
            if (model != null) NarrativeGen.Engine.LoadModel(model);
            foreach (var page in decoded)
            {
                if (page == null || page.blocks == null) throw new InvalidDataException("章の本文がありません。");
                ValidateBlocks(page.blocks, images.Count);
            }
            var textures = new Texture2D[images.Count];
            for (var i = 0; i < textures.Length; i++)
            {
                var bytes = Convert.FromBase64String((string)images[i]);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { name = "図版 " + (i + 1), wrapMode = TextureWrapMode.Clamp };
                if (!texture.LoadImage(bytes, true))
                {
                    DestroyImmediate(texture);
                    throw new InvalidDataException("図版 " + (i + 1) + " を読み込めません。");
                }
                textures[i] = texture;
                ctx.AddObjectToAsset("image-" + i, texture);
            }
            var book = ScriptableObject.CreateInstance<AdventureBook>();
            book.Initialize((string)source["title"], pages.ToString(Formatting.None), textures, source["style"]?.ToObject<AdventurePageStyle>(), model);
            ctx.AddObjectToAsset("book", book);
            ctx.SetMainObject(book);
        }

        private static void ValidateBlocks(IEnumerable<AdventureBlock> blocks, int imageCount)
        {
            foreach (var block in blocks)
            {
                if (block == null || string.IsNullOrWhiteSpace(block.kind))
                    throw new InvalidDataException("表示要素の種類がありません。");
                if (block.kind == "image" && (block.image < 0 || block.image >= imageCount))
                    throw new InvalidDataException("本文に対応する図版がありません。");
                if (block.children != null) ValidateBlocks(block.children, imageCount);
            }
        }
    }
}
