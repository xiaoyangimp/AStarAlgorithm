/// <summary>
/// Block node saved in the priority queue.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlNode {

	private Vector3 _pos;				// vector stores the point's position
	private float _distanceScore;		// discribes the distance score defined as distance between current point and destination
	private float _stepScore;			// discribe the step score, which takes one increment per step

	public BlNode() {
		_pos = new Vector3(0,0,0);
		_distanceScore = 0f;
		_stepScore = 0f;
	}

	public BlNode ( Vector3 v ) {
		_pos = v;
		_distanceScore = 0;
		_stepScore = 0;
	}

	public Vector3 pos {
		get { return _pos; }
		set { _pos = value; }
	}

	public float distanceScore {
		get { return _distanceScore; }
		set { _distanceScore = value; }
	}

	public float stepScore {
		get { return _stepScore; }
		set { _stepScore = value; }
	}

	/// <summary>
	/// Totals the distance score and step score.
	/// </summary>
	/// <returns>The score.</returns>
	public float TotalScore () {
		return _distanceScore + _stepScore;
	}

	/// <summary>
	/// Gets the distance score, use score = |x1-x2| + |y1-y2|
	/// </summary>
	/// <returns>The distance score.</returns>
	/// <param name="v">V.</param>
	public float GetDistanceScore( Vector3 v) {
		return  ( Mathf.Abs(  _pos.x - v.x ) + Mathf.Abs( _pos.y - v.y ) );
	}
}
