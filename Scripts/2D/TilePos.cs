using UnityEngine;

namespace M8 {
    public struct TilePos {
        public int row;
        public int col;

        public static TilePos zero { get { return new TilePos() { row = 0, col = 0 }; } }
        public static TilePos one { get { return new TilePos() { row = 1, col = 1 }; } }

        public Vector2 ToVector2(Vector2 size) {
            return new Vector2(col * size.x, row * size.y);
        }

        public Vector2 ToVector2(float width, float height) {
            return new Vector2(col * width, row * height);
        }

        public TilePos(int r, int c) {
            row = r;
            col = c;
        }

        public TilePos(Vector2 v, Vector2 size) {
            col = Mathf.FloorToInt(v.x / size.x);
            row = Mathf.FloorToInt(v.y / size.y);
        }

        public TilePos(Vector2 v, float width, float height) {
            col = Mathf.FloorToInt(v.x / width);
            row = Mathf.FloorToInt(v.y / height);
        }

        public TilePos(float x, float y, Vector2 size) {
            col = Mathf.FloorToInt(x / size.x);
            row = Mathf.FloorToInt(y / size.y);
        }

        public TilePos(float x, float y, float width, float height) {
            col = Mathf.FloorToInt(x / width);
            row = Mathf.FloorToInt(y / height);
        }

        public override string ToString() {
            return string.Format("[c={0}, r={1}]", col, row);
        }
    }
}