using UnityEngine.UI;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using System;

namespace ET
{
    public class SpriteValue : Entity
    {
        public Sprite asset;
        public int ref_count;
    }

    public class SpriteAtlasValue : Entity
    {
        public Dictionary<string, SpriteValue> subasset;
        public SpriteAtlas asset;
        public int ref_count;
    }
    
    public class ImageLoaderComponent : Entity,IAwake,IDestroy
    {
        public readonly string ATLAS_KEY = "/Atlas/";
        public Type sprite_type = typeof(Sprite);
        public Type sprite_atlas_type = typeof(SpriteAtlas);
        public static ImageLoaderComponent Instance { get; set; }

        public LruCache<string, SpriteValue> m_cacheSingleSprite;

        public LruCache<string, SpriteAtlasValue> m_cacheSpriteAtlas;

    }


}
