/// <summary>
/// Generate map class that attached to the GameMaster for A* algorithm.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMap : MonoBehaviour {
	
	public enum BlockType {					// enums used for describing map array elements
		blankgroundblock,					
		blockingblock,
		destinationblock,
		sourceblock,
		visitedblock,
		pretendedblock
	}

	public int NUMBER = 30;			// number of rows and columns in the map
	public int NUMBER_OF_BLOCKS = 36;// number of blocking walls

	public GameObject unitblock;			// to store the blank block

	private Color blankground = Color.white;
	private Color blocking = Color.black;
	private Color destination = Color.red;
	private Color source = Color.green;
	private Color visited = Color.grey;
	private Color pretended = Color.blue;

	private BlockType[,] _mapInformation;	// an array to store all the block information
	private GameObject[,] _blockArray;		// an array to hold all the block gameObjects
	private PriorityQueue<BlNode> pqueue; 	// priority queue for used for A* algorithm

	public float waitSecond = 0.5f;			// interval between block showing
	public int minWallLength = 2;			// the minimum length of the blocking wall
	public int maxWallLength = 6;			// the maximum length of the blocking wall + 1, used in Random.Range's Max

	private Vector3 dest;					// destination block position
	private Vector3 sour;					// source block position

	public Camera myCamera;					// cached camera reference


	private bool _solved = false;			// a flag indicating whether the problem is solved



	// Use this for initialization
	void Start () {

		// initializate the block information and gamObject arrays
		_mapInformation = new BlockType[NUMBER,NUMBER];
		_blockArray = new GameObject[NUMBER,NUMBER];

		// initializate the priority queue
		pqueue = new PriorityQueue<BlNode>();

		initializeArray();

		for( int i = 0; i < NUMBER; i++ ) {		// initialize that all the surrounding 
			_mapInformation[0, i] = BlockType.blockingblock;
			_mapInformation[NUMBER - 1, i] = BlockType.blockingblock;
			_mapInformation[i, 0] = BlockType.blockingblock;
			_mapInformation[i, NUMBER - 1] = BlockType.blockingblock;
		}

		// relocate the cached camera
		myCamera.transform.position = new Vector3 ( NUMBER / 2, NUMBER / 2, -10 );
		myCamera.orthographicSize = NUMBER / 2;

		// show all blocks
		ShowBlocks();

		// initialize the BlNode for the source and saved into the priority queue
		BlNode st = new BlNode( sour );
		st.stepScore = 0;
		st.distanceScore = st.GetDistanceScore( dest );
		pqueue.Enqueue( st );

		// start the solve coroutine (show details for each step)
		StartCoroutine( Solve () );
	}

	/// <summary>
	/// Solve the map by A*.
	/// </summary>
	IEnumerator Solve() {



		while( !_solved ) {

			BlNode cur = pqueue.Dequeue();

			if( cur == null ) break;

			float _x = cur.pos.x;
			float _y = cur.pos.y;

			_mapInformation[ (int) _x, (int)_y ] = BlockType.visitedblock;


			if( _x + 1 < NUMBER && !isBlock( _x + 1, _y )) {	// go one step right
				if(_mapInformation[ (int) (_x + 1), (int)_y ] != BlockType.visitedblock) {
					pqueue.Enqueue ( CreateNewNode( _x + 1, _y, cur ) );
					_mapInformation[ (int) (_x + 1), (int)_y ] = BlockType.pretendedblock;
				}
			}

			if( _x - 1 > 0 && !isBlock( _x - 1, _y )) {			// go one step left
				if(_mapInformation[ (int) (_x - 1), (int)_y ] != BlockType.visitedblock) {
					pqueue.Enqueue ( CreateNewNode( _x - 1, _y, cur ) );
					_mapInformation[ (int) (_x - 1), (int)_y ] = BlockType.pretendedblock;
				}
			}

			if( _y + 1 < NUMBER && !isBlock( _x, _y + 1 )) {	// go one step up
				if(_mapInformation[ (int)_x, (int) (_y + 1) ] != BlockType.visitedblock) {
					pqueue.Enqueue ( CreateNewNode( _x, _y + 1, cur ) );
					_mapInformation[ (int)_x, (int) (_y + 1) ] = BlockType.pretendedblock;
				}
			}

			if( _y - 1 > 0 && !isBlock( _x, _y - 1  ) ) {		// go one step down
				if(_mapInformation[ (int)_x, (int) (_y - 1) ] != BlockType.visitedblock) {
					pqueue.Enqueue ( CreateNewNode( _x, _y - 1, cur ) );
					_mapInformation[ (int)_x, (int) (_y - 1) ] = BlockType.pretendedblock;
				}
			}

			if( _mapInformation[ (int)dest.x, (int)dest.y ] == BlockType.visitedblock ) {	//determine whether or not it reaches the destination
				_solved = true;
			}

			_mapInformation[ (int) sour.x, (int) sour.y] = BlockType.sourceblock;			// remark the source block for coloring

			ShowBlocks();

			//yield return new WaitForSeconds(waitSecond);
			yield return null;
		}

		_mapInformation[ (int)dest.x, (int)dest.y ] = BlockType.destinationblock;
		_mapInformation[ (int)sour.x, (int)sour.y ] = BlockType.sourceblock;

		if( !_solved) 
			Debug.Log( "Unsolvable");

		ShowBlocks();
	}

	/// <summary>
	/// Initializes the array, destination and source.
	/// </summary>
	private void initializeArray() {

		// initialize all blocks
		for( int i = 0; i < NUMBER; i++ ) {
			
			for( int k = 0; k < NUMBER; k++ ) {
				
				_mapInformation[i, k] = BlockType.blankgroundblock;
				
			}
		}

		// choose the destination and source points, with minimum distance limit
		do {
			dest = new Vector3( Random.Range (1, NUMBER - 1), Random.Range (1, NUMBER - 1), 0);
			sour = new Vector3( Random.Range (1, NUMBER - 1), Random.Range (1, NUMBER - 1), 0);
		} while ( Mathf.Abs (dest.x - sour.x) + Mathf.Abs (dest.y - sour.y) < NUMBER );

		// create the source and destination
		_mapInformation[(int)sour.x, (int)sour.y] = BlockType.sourceblock;
		_mapInformation[(int)dest.x, (int)dest.y] = BlockType.destinationblock;

		// create several blocking walls
		for( int k = 0; k < NUMBER_OF_BLOCKS; k++ ) {

			Vector3 seed = new Vector3( Random.Range ( maxWallLength - 1, NUMBER - maxWallLength + 1) , Random.Range ( maxWallLength - 1, NUMBER - maxWallLength + 1 ) , 0 );
			int direction = (int) Random.Range (0, 4);

			GenerateWall(seed, direction);

		}


		// create the gameObjects of blocks
		for( int i = 0; i < NUMBER; i++ ) {
			
			for( int k = 0; k < NUMBER; k++ ) {

				Vector3 position = new Vector3 ( 0.5f + i, 0.5f + k, 0f );

				_blockArray[i, k] = Instantiate( unitblock, position, Quaternion.identity ) as GameObject;
				
			}
		}

	}

	/// <summary>
	/// Shows all the blocks.
	/// </summary>
	private void ShowBlocks() {	

		for( int i = 0; i < NUMBER; i++ ) {

			for( int k = 0; k < NUMBER; k++ ) {

				ShowBlock( i, k );

			}
		}
	}

	/// <summary>
	/// Shows each block.
	/// </summary>
	/// <param name="row">Row index of the block</param>
	/// <param name="column">Column index of the block.</param>
	private void ShowBlock( int row, int column ) {

		// Color the blocks
		switch( _mapInformation[row, column] ) {
		case BlockType.blockingblock:
			_blockArray[row,column].GetComponent<SpriteRenderer>().color = blocking;
			break;
		case BlockType.blankgroundblock:
			_blockArray[row,column].GetComponent<SpriteRenderer>().color = blankground;
			break;
		case BlockType.destinationblock:
			_blockArray[row,column].GetComponent<SpriteRenderer>().color = destination;
			break;
		case BlockType.sourceblock:
			_blockArray[row,column].GetComponent<SpriteRenderer>().color = source;
			break;
		case BlockType.visitedblock:
			_blockArray[row,column].GetComponent<SpriteRenderer>().color = visited;
			break;
		case BlockType.pretendedblock:
			_blockArray[row,column].GetComponent<SpriteRenderer>().color = pretended;
			break;
		default:
			Debug.Log ("Unplanned int value");
			break;
		}

	}

	/// <summary>
	/// Creates a New Node.
	/// </summary>
	/// <returns>The nearby block.</returns>
	/// <param name="x">The x coordinate of new node.</param>
	/// <param name="y">The y coordinate of new node.</param>
	/// <param name="originalNode">The original node, which is a nearby node to this new node.</param>
	private BlNode CreateNewNode( float x, float y, BlNode originalNode ) {
		BlNode t = new BlNode( new Vector3( x, y, 0 ) );
		t.distanceScore = t.GetDistanceScore( dest );
		t.stepScore = originalNode.stepScore + 1;

		return t;
	}

	/// <summary>
	/// Determines whether the current block is a blocking wall.
	/// </summary>
	/// <returns><c>true</c>, if block was ised, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private bool isBlock( float x, float y ) {
		if( _mapInformation[ (int) x, (int) y ] == BlockType.blockingblock)
		   	return true;
		else 
			return false;
	}


	/// <summary>
	/// Generates the blocking wall.
	/// </summary>
	/// <param name="seed">Seed of the wall</param>
	/// <param name="direction">Direction of the wall</param>
	private void GenerateWall(Vector3 seed, int direction) {

		// determine the length of the wall
		int RandomWallNumber = Random.Range ( minWallLength, maxWallLength );

		switch( direction ) {
		case 0:	// Left

			for( int i= 0; i < (int) RandomWallNumber; i++ ) {
				if( _mapInformation[ (int) ( seed.x - i ) , (int) seed.y] == BlockType.blankgroundblock )
					_mapInformation[ (int) ( seed.x - i ) , (int) seed.y] = BlockType.blockingblock;
			}

			break;
		case 1: //Up

			for( int i= 0; i < (int) RandomWallNumber; i++ ) {
				if( _mapInformation[ (int) seed.x , (int) ( seed.y + i )] == BlockType.blankgroundblock )
					_mapInformation[ (int) seed.x , (int) ( seed.y + i ) ] = BlockType.blockingblock;
			}

			break;
		case 2:	//Right

			for( int i= 0; i < (int) RandomWallNumber; i++ ) {
				if( _mapInformation[ (int) ( seed.x + i ) , (int) seed.y] == BlockType.blankgroundblock )
					_mapInformation[ (int) ( seed.x + i ) , (int) seed.y] = BlockType.blockingblock;
			}

			break;
		case 3:	//down

			for( int i= 0; i < (int) RandomWallNumber; i++ ) {
				if( _mapInformation[ (int) seed.x , (int) ( seed.y - i )] == BlockType.blankgroundblock )
					_mapInformation[ (int) seed.x , (int) ( seed.y - i )] = BlockType.blockingblock;
			}

			break;
		default:
			Debug.Log ("Unexpected direction!");
			break;

		}

	}
}
	