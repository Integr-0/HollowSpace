/*
 * This script is a slightly modified version of the script provided by Unity Technologies
 * The original script can be found here: https://docs.unity3d.com/6000.0/Documentation/Manual/tilemaps/tiles-for-tilemaps/scriptable-tiles/create-scriptable-tile.html
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Tiles/Electrical Wire Tile")]
public class ElectricalWireTile : TileBase {
    public Sprite[] unpoweredSprites;
    public Sprite[] poweredSprites;

    private readonly Dictionary<Vector3Int, bool> _poweredTiles = new();

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        _poweredTiles.TryAdd(position, false);
        return true;
    }

    /// <summary>
    /// This method is used to power a tile.
    /// </summary>
    /// <param name="position">The position of the Tile to power</param>
    /// <param name="powered">The state to write to the wire</param>
    /// <param name="powerChain">The chain of tiles powering each other that led up to this one</param>
    public void PowerTile(ITilemap tilemap, Vector3Int position, bool powered, LinkedList<Vector3Int> powerChain = null) {
        _poweredTiles[position] = powered;

        powerChain ??= new LinkedList<Vector3Int>();
        Debug.Log($"Powering tile at {position} to {powered} (chain: {string.Join(", ", powerChain)})");
        powerChain.AddLast(position);
        
        tilemap.RefreshTile(position);

        for (int yd = -1; yd <= 1; yd++)
        for (int xd = -1; xd <= 1; xd++) {
            Vector3Int pos = new(position.x + xd, position.y + yd, position.z);

            if (!IsSameTile(tilemap, pos)) continue;
            if (powerChain.Contains(pos)) continue;

            PowerTile(tilemap, pos, powered, powerChain);
        }
    }
    
    /// <summary>
    /// Gets the power state of a tile.
    /// </summary>
    /// <param name="position">The position of the tile.</param>
    /// <returns>If the tile is powered or not.</returns>
    public bool IsPowered(Vector3Int position) {
        return _poweredTiles.ContainsKey(position) && _poweredTiles[position];
    }
    
    public void Toggle(ITilemap tilemap, Vector3Int position) {
        PowerTile(tilemap, position, !IsPowered(position));
    }

    /// <summary>
    /// This method is called when the tile is refreshed. We will refresh all tiles around this tile.
    /// </summary>
    /// <param name="position">Position of the tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
        for (int yd = -1; yd <= 1; yd++)
        for (int xd = -1; xd <= 1; xd++) {
            Vector3Int pos = new(position.x + xd, position.y + yd, position.z);

            if (IsSameTile(tilemap, pos)) {
                tilemap.RefreshTile(pos);
            }
        }
    }


    /// <summary>
    /// Retrieves any tile rendering data from the scripted tile.
    /// </summary>
    /// <param name="position">Position of the tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    /// <param name="tileData">Data to render the tile.</param>
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        UpdateTile(position, tilemap, ref tileData);
    }


    /// <summary>
    /// Checks the orthogonal neighbouring positions of the tile and generates a mask based on whether the neighboring tiles are the same.
    /// The mask will determine the according Sprite and transform to be rendered at the given position.
    /// The Sprite and Transform is then filled into TileData for the Tilemap to use.
    /// The Flags lock the color and transform to the data provided by the tile.
    /// The ColliderType is set to the shape of the Sprite used.
    /// </summary>
    private void UpdateTile(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.transform = Matrix4x4.identity;
        tileData.color = Color.white;


        int mask = IsSameTile(tilemap, position + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += IsSameTile(tilemap, position + new Vector3Int(1, 0, 0)) ? 2 : 0;
        mask += IsSameTile(tilemap, position + new Vector3Int(0, -1, 0)) ? 4 : 0;
        mask += IsSameTile(tilemap, position + new Vector3Int(-1, 0, 0)) ? 8 : 0;


        int index = GetIndex((byte)mask);
        if (index >= 0 && index < poweredSprites.Length && IsSameTile(tilemap, position)) {
            tileData.sprite = IsPowered(position) ? poweredSprites[index] : unpoweredSprites[index];
            tileData.transform = GetTransform((byte)mask);
            tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.Sprite;
        }
    }


    /// <summary>
    /// Determines if the tile at the given position is the same tile as this.
    /// </summary>
    private bool IsSameTile(ITilemap tileMap, Vector3Int position) {
        TileBase tile = tileMap.GetTile(position);
        return tile != null && tile == this;
    }

    /*
        Mental notes:

        Given sprites:
        0: None
        1: One (left)
        2: Two (top-bottom)
        3: Two (top-left)
        4: Three (left-right-top)
        5: Four

        Mask:
        0: No neighbours

        1: One neighbour (on the top)
        2: One neighbour (on the right)
        4: One neighbour (on the bottom)
        8: One neighbour (on the left)

        3: Two neighbours (corner) (top and right)
        9: Two neighbours (corner) (top and left)
        6: Two neighbours (corner) (bottom and right)
        12: Two neighbours (corner) (bottom and left)

        5: Two neighbours (top and bottom)
        10: Two neighbours (right and left)

        7: Three neighbours (top, right and bottom)
        11: Three neighbours (top, right and left)
        13: Three neighbours (top, bottom and left)
        14: Three neighbours (right, bottom and left)

        15: Four neighbours

        Transforms (counterclockwise):
        0: none

        1: 90
        2: 180
        4: 270
        8: none

        3: 90
        6: 180
        9: none
        12: 270

        5: none
        10: 90

        7: 90
        11: none
        13: 270
        14: 180

        15: none
     */

    /// <summary>
    /// Determines the index of the Sprite to be used based on the neighbour mask.
    /// </summary>
    private int GetIndex(byte mask) {
        switch (mask) {
            case 0:
                return 0;

            case 1:
            case 2:
            case 4:
            case 8:
                return 1;

            case 5:
            case 10:
                return 2;

            case 3:
            case 6:
            case 9:
            case 12:
                return 3;

            case 7:
            case 11:
            case 13:
            case 14:
                return 4;

            case 15:
                return 5;
        }

        return -1;
    }


    /// <summary>
    /// Determines the Transform (rotation in this case) to be used based on the neighbour mask.
    /// </summary>
    private Matrix4x4 GetTransform(byte mask) {
        switch (mask) {
            case 1:
            case 3:
            case 10:
            case 7:
                return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);

            case 2:
            case 6:
            case 14:
                return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one);

            case 4:
            case 12:
            case 13:
                return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
        }

        return Matrix4x4.identity;
    }
}

#if UNITY_EDITOR
/// <summary>
/// Custom Editor for a PipelineExampleTile. This is shown in the Inspector window when a PipelineExampleTile asset is selected.
/// </summary>
[CustomEditor(typeof(ElectricalWireTile))]
public class PipelineExampleTileEditor : Editor {
    private ElectricalWireTile tile => target as ElectricalWireTile;


    public void OnEnable() {
        if (tile.unpoweredSprites == null || tile.unpoweredSprites.Length != 6)
            tile.unpoweredSprites = new Sprite[6];

        if (tile.poweredSprites == null || tile.poweredSprites.Length != 6)
            tile.poweredSprites = new Sprite[6];
    }


    /// <summary>
    /// Draws an Inspector for the PipelineExampleTile.
    /// </summary>
    public override void OnInspectorGUI() {
        EditorGUILayout.LabelField("Place sprites shown based on the number of tiles bordering it and the specified connections.");
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Unpowered Sprites");

        EditorGUI.BeginChangeCheck();
        tile.unpoweredSprites[0] = (Sprite)EditorGUILayout.ObjectField("None", tile.unpoweredSprites[0], typeof(Sprite), false, null);
        tile.unpoweredSprites[1] = (Sprite)EditorGUILayout.ObjectField("One (left)", tile.unpoweredSprites[1], typeof(Sprite), false, null);
        tile.unpoweredSprites[2] = (Sprite)EditorGUILayout.ObjectField("Two (top-bottom)", tile.unpoweredSprites[2], typeof(Sprite), false, null);
        tile.unpoweredSprites[3] = (Sprite)EditorGUILayout.ObjectField("Two (top-left)", tile.unpoweredSprites[3], typeof(Sprite), false, null);
        tile.unpoweredSprites[4] = (Sprite)EditorGUILayout.ObjectField("Three (left-right-top)", tile.unpoweredSprites[4], typeof(Sprite), false, null);
        tile.unpoweredSprites[5] = (Sprite)EditorGUILayout.ObjectField("Four", tile.unpoweredSprites[5], typeof(Sprite), false, null);
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(tile);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Powered Sprites");

        EditorGUI.BeginChangeCheck();
        tile.poweredSprites[0] = (Sprite)EditorGUILayout.ObjectField("None", tile.poweredSprites[0], typeof(Sprite), false, null);
        tile.poweredSprites[1] = (Sprite)EditorGUILayout.ObjectField("One (left)", tile.poweredSprites[1], typeof(Sprite), false, null);
        tile.poweredSprites[2] = (Sprite)EditorGUILayout.ObjectField("Two (top-bottom)", tile.poweredSprites[2], typeof(Sprite), false, null);
        tile.poweredSprites[3] = (Sprite)EditorGUILayout.ObjectField("Two (top-left)", tile.poweredSprites[3], typeof(Sprite), false, null);
        tile.poweredSprites[4] = (Sprite)EditorGUILayout.ObjectField("Three (left-right-top)", tile.poweredSprites[4], typeof(Sprite), false, null);
        tile.poweredSprites[5] = (Sprite)EditorGUILayout.ObjectField("Four", tile.poweredSprites[5], typeof(Sprite), false, null);
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(tile);
        }
    }
}
#endif