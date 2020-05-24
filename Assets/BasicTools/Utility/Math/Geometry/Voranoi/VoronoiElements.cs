/*
	  Copyright 2011 James Humphreys. All rights reserved.
	
	Redistribution and use in source and binary forms, with or without modification, are
	permitted provided that the following conditions are met:
	
	   1. Redistributions of source code must retain the above copyright notice, this list of
	      conditions and the following disclaimer.
	
	   2. Redistributions in binary form must reproduce the above copyright notice, this list
	      of conditions and the following disclaimer in the documentation and/or other materials
	      provided with the distribution.
	
	THIS SOFTWARE IS PROVIDED BY James Humphreys ``AS IS'' AND ANY EXPRESS OR IMPLIED
	WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
	FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
	CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
	CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
	SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
	ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
	ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	
	The views and conclusions contained in the software and documentation are those of the
	authors and should not be interpreted as representing official policies, either expressed
	or implied, of James Humphreys.
 */

using System.Collections.Generic;
using UnityEngine;

namespace BasicTools.Utility
{
	// use for sites and vertecies
	public class Site
	{
		public Vector2 coord;
		public int sitenbr;
		
		public Site ()
		{
			coord = new Vector2();
		}
	}
	
	public class Edge
	{
		public float a = 0, b = 0, c = 0;
		public Site[] ep;
		public Site[] reg;
		public int edgenbr;
		
		public Edge ()
		{
			ep = new Site[2];
			reg = new Site[2];
		}
	}
	
	
	public class Halfedge
	{
		public Halfedge ELleft, ELright;
		public Edge ELedge;
		public bool deleted;
		public int ELpm;
		public Site vertex;
		public float ystar;
		public Halfedge PQnext;
		
		public Halfedge ()
		{
			PQnext = null;
		}
	}
	
	public class GraphEdge
	{
		public float x1, y1, x2, y2;
		public int site1, site2;
	}
	

	public class SiteSorterYX : IComparer<Site>
	{
		public int Compare ( Site p1, Site p2 )
		{
			Vector2 s1 = p1.coord;
			Vector2 s2 = p2.coord;
			if ( s1.y < s2.y )	return -1;
			if ( s1.y > s2.y ) return 1;
			if ( s1.x < s2.x ) return -1;
			if ( s1.x > s2.x ) return 1;
			return 0;
		}
	}
}
