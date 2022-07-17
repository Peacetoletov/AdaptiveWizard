using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptiveWizard.Assets.Scripts.Other.GameManagers;


namespace AdaptiveWizard.Assets.Scripts.Other.Other
{
    public static class Utility
    {
        public static Vector2 SpriteSize(string pathToSprite) {
            // Returns the size of a sprite as a 2D vector (width x height) (not in pixels, rather as a multiple of the base unit, currently 16 pixels)
            Sprite sprite = Resources.Load<Sprite>(pathToSprite);
            return new Vector2(sprite.rect.width, sprite.rect.height) / MainGameManager.PIXELS_PER_UNIT;
        }
    }
}
