/// <summary>
/// An simple implementation for the priority queue.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T> : List<T> where T : BlNode {

	private List<T> pqueue;
	private int numberOfElements;

	/// <summary>
	/// Initializes a new instance of the <see cref="PriorityQueue`1"/> class.
	/// </summary>
	public PriorityQueue () {

		pqueue = new List<T>();
		numberOfElements = 0;

	}

	/// <summary>
	/// Enqueue the specified block.
	/// </summary>
	/// <param name="block">Block.</param>
	public void Enqueue (T block) {

		if( numberOfElements == 0 ) {
			pqueue.Add ( block );
			numberOfElements++;
			return;
		}

		int cnt;

		for( cnt = 0; cnt < numberOfElements; cnt++) {

			if(  block.TotalScore() <= pqueue[cnt].TotalScore() ) {
				break;
			}
		}

		pqueue.Insert( cnt, block );
		numberOfElements++;
	}

	/// <summary>
	/// Dequeue this instance.
	/// </summary>
	public T Dequeue() {
		if( numberOfElements <= 0 ) return null;

		T ret = pqueue[0];

		pqueue.RemoveAt ( 0 );

		numberOfElements--;

		return ret;
	}


}
