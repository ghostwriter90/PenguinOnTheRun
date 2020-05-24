 /*
 * The author of this software is Steven Fortune.  Copyright (c) 1994 by AT&T
 * Bell Laboratories.
 * Permission to use, copy, modify, and distribute this software for any
 * purpose without fee is hereby granted, provided that this entire notice
 * is included in all copies of any software which is or includes a copy
 * or modification of this software and in all copies of the supporting
 * documentation for such software.
 * THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTY.  IN PARTICULAR, NEITHER THE AUTHORS NOR AT&T MAKE ANY
 * REPRESENTATION OR WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY
 * OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.
 */

/* 
 * This code was originally written by Stephan Fortune in C code.  I, Shane O'Sullivan,
 * have since modified it, encapsulating it in a C++ class and, fixing memory leaks and
 * adding accessors to the Voronoi Edges.
 * Permission to use, copy, modify, and distribute this software for any
 * purpose without fee is hereby granted, provided that this entire notice
 * is included in all copies of any software which is or includes a copy
 * or modification of this software and in all copies of the supporting
 * documentation for such software.
 * THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTY.  IN PARTICULAR, NEITHER THE AUTHORS NOR AT&T MAKE ANY
 * REPRESENTATION OR WARRANTY OF ANY KIND CONCERNING THE MERCHANTABILITY
 * OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.
 */


using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicTools.Utility
{
	/// <summary>
	/// Description of Voronoi.
	/// </summary>
	public class Voronoi
	{
        // ************* members ******************
        Vector2 borderMin = new Vector2();
        Vector2 borderMax = new Vector2();
        int siteidx;
		float xmin, xmax, ymin, ymax, deltax, deltay;
		int nvertices;
		int nedges;
		int nsites;
		Site[] sites;
		Site bottomsite;
		int sqrt_nsites;
		float minDistanceBetweenSites;
		int PQcount;
		int PQmin;
		int PQhashsize;
		Halfedge[] PQhash;
		
		const int LE = 0;
		const int RE = 1;

		int ELhashsize;
		Halfedge[] ELhash;
		Halfedge ELleftend, ELrightend;
		List<GraphEdge> allEdges;
		
		
		// ************* Public methods ******************
		// ******************************************
		
		// constructor
		public Voronoi ( float minDistanceBetweenSites )
		{
			siteidx = 0;
			sites = null;
			
			allEdges = null;
			this.minDistanceBetweenSites = minDistanceBetweenSites;
		}
		
		/**
		 * 
		 * @param xValuesIn Array of X values for each site.
		 * @param yValuesIn Array of Y values for each site. Must be identical length to yValuesIn
		 * @param minX The minimum X of the bounding box around the voronoi
		 * @param maxX The maximum X of the bounding box around the voronoi
		 * @param minY The minimum Y of the bounding box around the voronoi
		 * @param maxY The maximum Y of the bounding box around the voronoi
		 * @return
		 */
		public List<GraphEdge> GenerateVoronoi ( Vector2[] positions ,Vector2 min, Vector2 max )
		{
            Sort(positions);

            // Check bounding box inputs - if mins are bigger than maxes, swap them
            float temp = 0;
            if (min.x > max.x)
            {
                temp = min.x;
                min.x = max.x;
                max.x = temp;
            }
            if (min.y > max.y)
            {
                temp = min.y;
                min.y = max.y;
                max.y = temp;
            }

            borderMin.x = min.x;
            borderMin.y = min.y;
            borderMax.x = max.x;
            borderMax.y = max.y;

            siteidx = 0;
            Voronoi_bd();
            return allEdges;
        }
		
		
		/*********************************************************
		 * methods - implementation details
		 ********************************************************/
		
		void Sort (Vector2[] positions)
		{
            int count = positions.Length;
			sites = null;
			allEdges = new List<GraphEdge>();
			
			nsites = count;
			nvertices = 0;
			nedges = 0;
			
			float sn = (float)nsites + 4;
			sqrt_nsites = (int) Math.Sqrt ( sn );

            // Copy the inputs so we don't modify the originals
            Vector2[] outValues = new Vector2[count];
			for (int i = 0; i < count; i++)
			{
                outValues[i] = positions[i];
			}
			SortNode (outValues );
		}
		
		void SortNode (Vector2[] Values )
        {
            int count = Values.Length;
			sites = new Site[nsites];
			xmin = Values[0].x;
			ymin = Values[0].y;
			xmax = Values[0].x;
			ymax = Values[0].y;
			
			for ( int i = 0; i < nsites; i++ )
			{
				sites[i] = new Site();
                sites[i].coord.x = Values[i].x;
                sites[i].coord.y = Values[i].y;  // ???     sites[i].coord.x = Values[i].y; 
                sites[i].sitenbr = i;
				
				if ( Values[i].x < xmin )
					xmin = Values[i].x;
				else if ( Values[i].x > xmax )
					xmax = Values[i].x;
				
				if ( Values[i].y < ymin )
					ymin = Values[i].y;
				else if ( Values[i].y > ymax )
					ymax = Values[i].y;
			}
			
			Qsort ( sites );
			deltax = xmax - xmin;
			deltay = ymax - ymin;
		}

        void Qsort ( Site[] sites )
		{
			List<Site> listSites = new List<Site>( sites.Length );
			for ( int i = 0; i < sites.Length; i++ )
			{
				listSites.Add ( sites[i] );
			}
			
			listSites.Sort ( new SiteSorterYX () );
			
			// Copy back into the array
			for (int i=0; i < sites.Length; i++)
			{
				sites[i] = listSites[i];
			}
		}

		Site NextOne ()
		{
			Site s;
			if ( siteidx < nsites )
			{
				s = sites[siteidx];
				siteidx++;
				return s;
			}
			return null;
		}
		
		Edge Bisect ( Site s1, Site s2 )
		{
			float dx, dy, adx, ady;
			Edge newedge;
			
			newedge = new Edge();
			
			newedge.reg[0] = s1;
			newedge.reg[1] = s2;
			
			newedge.ep [0] = null;
			newedge.ep[1] = null;
			
			dx = s2.coord.x - s1.coord.x;
			dy = s2.coord.y - s1.coord.y;
			
			adx = dx > 0 ? dx : -dx;
			ady = dy > 0 ? dy : -dy;
			newedge.c = (float)(s1.coord.x * dx + s1.coord.y * dy + (dx * dx + dy* dy) * 0.5);
			
			if ( adx > ady )
			{
				newedge.a = 1.0f;
				newedge.b = dy / dx;
				newedge.c /= dx;
			}
			else
			{
				newedge.a = dx / dy;
				newedge.b = 1.0f;
				newedge.c /= dy;
			}
			
			newedge.edgenbr = nedges;
			nedges++;
			
			return newedge;
		}
		
		void MakeVertex ( Site v )
		{
			v.sitenbr = nvertices;
			nvertices++;
		}
		
		bool PQ_Initialize ()
		{
			PQcount = 0;
			PQmin = 0;
			PQhashsize = 4 * sqrt_nsites;
			PQhash = new Halfedge[ PQhashsize ];
			
			for ( int i = 0; i < PQhashsize; i++ )
			{
				PQhash [i] = new Halfedge();
			}
			return true;
		}
		
		int PQ_Bucket ( Halfedge he )
		{
			int bucket;
			
			bucket = (int) ((he.ystar - ymin) / deltay * PQhashsize);
			if ( bucket < 0 )
				bucket = 0;
			if ( bucket >= PQhashsize )
				bucket = PQhashsize - 1;
			if ( bucket < PQmin )
				PQmin = bucket;
			
			return bucket;
		}
		
		// push the HalfEdge into the ordered linked list of vertices
		void PQ_Insert ( Halfedge he, Site v, float offset )
		{
			Halfedge last, next;
			
			he.vertex = v;
			he.ystar = (float)(v.coord.y + offset);
			last = PQhash [ PQ_Bucket (he) ];
			
			while
				(
					(next = last.PQnext) != null
					&& 
					(he.ystar > next.ystar || (he.ystar == next.ystar && v.coord.x > next.vertex.coord.x))
				)
			{
				last = next;
			}
			
			he.PQnext = last.PQnext;
			last.PQnext = he;
			PQcount++;
		}
		
		// remove the HalfEdge from the list of vertices
		void PQ_Delete ( Halfedge he )
		{
			Halfedge last;
			
			if (he.vertex != null)
			{
				last = PQhash [ PQ_Bucket (he) ];
				while ( last.PQnext != he )
				{
					last = last.PQnext;
				}
				
				last.PQnext = he.PQnext;
				PQcount--;
				he.vertex = null;
			}
		}
		
		bool PQ_Empty ()
		{
			return ( PQcount == 0 );
		}
		
		Vector2 PQ_min ()
		{
			Vector2 answer = new Vector2 ();
			
			while ( PQhash[PQmin].PQnext == null  )
			{
				PQmin++;
			}
			
			answer.x = PQhash[PQmin].PQnext.vertex.coord.x;
			answer.y = PQhash[PQmin].PQnext.ystar;
			return answer;
		}
		
		Halfedge PQ_Extractmin ()
		{
			Halfedge curr;
			
			curr = PQhash[PQmin].PQnext;
			PQhash[PQmin].PQnext = curr.PQnext;
			PQcount--;
			
			return curr;
		}
		
		Halfedge CreateHalfEdge(Edge e, int pm)
		{
			Halfedge answer = new Halfedge();
			answer.ELedge = e;
			answer.ELpm = pm;
			answer.PQnext = null;
			answer.vertex = null;
			
			return answer;
		}
		
		bool EL_Initialize()
		{
			ELhashsize = 2 * sqrt_nsites;
			ELhash = new Halfedge[ELhashsize];
			
			for (int i = 0; i < ELhashsize; i++)
			{
				ELhash[i] = null;
			}
			
			ELleftend = CreateHalfEdge ( null, 0 );
			ELrightend = CreateHalfEdge ( null, 0 );
			ELleftend.ELleft = null;
			ELleftend.ELright = ELrightend;
			ELrightend.ELleft = ELleftend;
			ELrightend.ELright = null;
			ELhash[0] = ELleftend;
			ELhash[ELhashsize - 1] = ELrightend;
			
			return true;
		}
		
		
		Site LeftReg( Halfedge he )
		{
			if (he.ELedge == null)
			{
				return bottomsite;
			}
			return (he.ELpm == LE ? he.ELedge.reg[LE] : he.ELedge.reg[RE]);
		}
		
		void EL_Insert( Halfedge lb, Halfedge newHe )
		{
			newHe.ELleft = lb;
			newHe.ELright = lb.ELright;
			(lb.ELright).ELleft = newHe;
			lb.ELright = newHe;
		}
		
		/*
		 * This delete routine can't reclaim node, since Vector2ers from hash table
		 * may be present.
		 */
		void EL_Delete( Halfedge he )
		{
			(he.ELleft).ELright = he.ELright;
			(he.ELright).ELleft = he.ELleft;
			he.deleted = true;
		}
		
		/* Get entry from hash table, pruning any deleted nodes */
		Halfedge EL_GetHash( int b )
		{
			Halfedge he;
			if (b < 0 || b >= ELhashsize)
				return null;
			
			he = ELhash[b];
			if (he == null || !he.deleted )
				return he;
			
			/* Hash table Vector2s to deleted half edge. Patch as necessary. */
			ELhash[b] = null;
			return null;
		}
		
		Halfedge EL_LeftBound( Vector2 p )
		{
			int bucket;
			Halfedge he;
			
			/* Use hash table to get close to desired halfedge */
			// use the hash function to find the place in the hash map that this
			// HalfEdge should be
			bucket = (int) ((p.x - xmin) / deltax * ELhashsize);
			
			// make sure that the bucket position is within the range of the hash
			// array
			if ( bucket < 0 ) bucket = 0;
			if ( bucket >= ELhashsize )	bucket = ELhashsize - 1;
			
			he = EL_GetHash ( bucket );
			
			// if the HE isn't found, search backwards and forwards in the hash map
			// for the first non-null entry
			if ( he == null )
			{
				for ( int i = 1; i < ELhashsize; i++ )
				{
					if ( (he = EL_GetHash ( bucket - i ) ) != null )
						break;
					if ( (he = EL_GetHash ( bucket + i ) ) != null )
						break;
				}
			}
			
			/* Now search linear list of halfedges for the correct one */
			if ( he == ELleftend || ( he != ELrightend && RightOf (he, p) ) )
			{
				// keep going right on the list until either the end is reached, or
				// you find the 1st edge which the Vector2 isn't to the right of
				do
				{
					he = he.ELright;
				}
				while ( he != ELrightend && RightOf(he, p) );
				he = he.ELleft;
			}
			else
				// if the Vector2 is to the left of the HalfEdge, then search left for
				// the HE just to the left of the Vector2
			{
				do
				{
					he = he.ELleft;
				}
				while ( he != ELleftend && !RightOf(he, p) );
			}
			
			/* Update hash table and reference counts */
			if ( bucket > 0 && bucket < ELhashsize - 1)
			{
				ELhash[bucket] = he;
			}
			
			return he;
		}
		
		void PushGraphEdge( Site leftSite, Site rightSite, float x1, float y1, float x2, float y2 )
		{
			GraphEdge newEdge = new GraphEdge ();
			allEdges.Add ( newEdge );
			newEdge.x1 = x1;
			newEdge.y1 = y1;
			newEdge.x2 = x2;
			newEdge.y2 = y2;
			
			newEdge.site1 = leftSite.sitenbr;
			newEdge.site2 = rightSite.sitenbr;
		}
		
		void ClipLine( Edge e )
        {
            Vector2 pMin = borderMin;
            Vector2 pMax = borderMax;
            Site s1, s2;
			
			float x1 = e.reg[0].coord.x;
			float y1 = e.reg[0].coord.y;
			float x2 = e.reg[1].coord.x;
			float y2 = e.reg[1].coord.y;
			float x = x2- x1;
			float y = y2 - y1;
			
			// if the distance between the two Vector2s this line was created from is
			// less than the square root of 2 , then ignore it
			if ( Math.Sqrt ( (x*x) + (y*y) ) < minDistanceBetweenSites )
			{
				return;
			}

            if ( e.a == 1.0 && e.b >= 0.0 )
			{
				s1 = e.ep[1];
				s2 = e.ep[0];
			}
			else
			{
				s1 = e.ep[0];
				s2 = e.ep[1];
			}
			
			if ( e.a == 1.0 )
			{
				y1 = pMin.y;
				
				if ( s1 != null && s1.coord.y > pMin.y )
					y1 = s1.coord.y;
				if ( y1 > pMax.y )
					y1 = pMax.y;
				x1 = e.c - e.b * y1;
				y2 = pMax.y;
				
				if ( s2 != null && s2.coord.y < pMax.y )
					y2 = s2.coord.y;
				if ( y2 < pMin.y )
					y2 = pMin.y;
				x2 = e.c - e.b * y2;
				if ( ( (x1 > pMax.x) & (x2 > pMax.x) ) | ( (x1 < pMin.x) & (x2 < pMin.x) ) )
					return;
				
				if ( x1 > pMax.x )
				{
					x1 = pMax.x;
					y1 = ( e.c - x1 ) / e.b;
				}
				if ( x1 < pMin.x )
				{
					x1 = pMin.x;
					y1 = ( e.c - x1 ) / e.b;
				}
				if ( x2 > pMax.x )
				{
					x2 = pMax.x;
					y2 = ( e.c - x2 ) / e.b;
				}
				if ( x2 < pMin.x )
				{
					x2 = pMin.x;
					y2 = ( e.c - x2 ) / e.b;
				}
				
			}
			else
			{
				x1 = pMin.x;
				if ( s1 != null && s1.coord.x > pMin.x )
					x1 = s1.coord.x;
				if ( x1 > pMax.x )
					x1 = pMax.x;
				y1 = e.c - e.a * x1;
				
				x2 = pMax.x;
				if ( s2 != null && s2.coord.x < pMax.x )
					x2 = s2.coord.x;
				if ( x2 < pMin.x )
					x2 = pMin.x;
				y2 = e.c - e.a * x2;
				
				if (((y1 > pMax.y) & (y2 > pMax.y)) | ((y1 < pMin.y) & (y2 < pMin.y)))
					return;
				
				if ( y1 > pMax.y )
				{
					y1 = pMax.y;
					x1 = ( e.c - y1 ) / e.a;
				}
				if ( y1 < pMin.y )
				{
					y1 = pMin.y;
					x1 = ( e.c - y1 ) / e.a;
				}
				if ( y2 > pMax.y )
				{
					y2 = pMax.y;
					x2 = ( e.c - y2 ) / e.a;
				}
				if ( y2 < pMin.y )
				{
					y2 = pMin.y;
					x2 = ( e.c - y2 ) / e.a;
				}
			}
			
			PushGraphEdge ( e.reg[0], e.reg[1], x1, y1, x2, y2 );
		}
		
	    void EndVector2( Edge e, int lr, Site s )
		{
			e.ep[lr] = s;
			if ( e.ep[RE - lr] == null )
				return;
			ClipLine ( e );
		}
		
		/* returns true if p is to right of halfedge e */
		static bool RightOf(Halfedge el, Vector2 p)
		{
			Edge e;
			Site topsite;
			bool right_of_site;
			bool above, fast;
			float dxp, dyp, dxs, t1, t2, t3, yl;
			
			e = el.ELedge;
			topsite = e.reg[1];
			
			if ( p.x > topsite.coord.x )
				right_of_site = true;
			else
				right_of_site = false;
			
			if ( right_of_site && el.ELpm == LE )
				return true;
			if (!right_of_site && el.ELpm == RE )
				return false;
			
			if ( e.a == 1.0 )
			{
				dxp = p.x - topsite.coord.x;
				dyp = p.y - topsite.coord.y;
				fast = false;
				
				if ( (!right_of_site & (e.b < 0.0)) | (right_of_site & (e.b >= 0.0)) )
				{
					above = dyp >= e.b * dxp;
					fast = above;
				}
				else
				{
					above = p.x + p.y * e.b > e.c;
					if ( e.b < 0.0 )
						above = !above;
					if ( !above )
						fast = true;
				}
				if ( !fast )
				{
					dxs = topsite.coord.x - ( e.reg[0] ).coord.x;
					above = e.b * (dxp * dxp - dyp * dyp) 
					< dxs * dyp * (1.0 + 2.0 * dxp / dxs + e.b * e.b);
					
					if ( e.b < 0 )
						above = !above;
				}
			}
			else // e.b == 1.0
			{
				yl = e.c - e.a * p.x;
				t1 = p.y - yl;
				t2 = p.x - topsite.coord.x;
				t3 = yl - topsite.coord.y;
				above = t1 * t1 > t2 * t2 + t3 * t3;
			}
			return ( el.ELpm == LE ? above : !above );
		}
		
		Site RightReg(Halfedge he)
		{
			if (he.ELedge == null)
				// if this halfedge has no edge, return the bottom site (whatever
				// that is)
			{
				return (bottomsite);
			}

			// if the ELpm field is zero, return the site 0 that this edge bisects,
			// otherwise return site number 1
			return (he.ELpm == LE ? he.ELedge.reg[RE] : he.ELedge.reg[LE]);
		}
		
		float Distance( Site s, Site t )
		{
			float dx, dy;
			dx = s.coord.x - t.coord.x;
			dy = s.coord.y - t.coord.y;
			return Mathf.Sqrt ( dx * dx + dy * dy );
		}
		
		// create a new site where the HalfEdges el1 and el2 intersect - note that
		// the Vector2 in the argument list is not used, don't know why it's there
		Site Intersect( Halfedge el1, Halfedge el2 )
		{
			Edge e1, e2, e;
			Halfedge el;
			float d, xint, yint;
			bool right_of_site;
			Site v; // vertex
			
			e1 = el1.ELedge;
			e2 = el2.ELedge;
			
			if ( e1 == null || e2 == null )
				return null;
			
			// if the two edges bisect the same parent, return null
			if ( e1.reg[1] == e2.reg[1] )
				return null;
			
			d = e1.a * e2.b - e1.b * e2.a;
			if ( -1.0e-10 < d && d < 1.0e-10 )
				return null;
			
			xint = ( e1.c * e2.b - e2.c * e1.b ) / d;
			yint = ( e2.c * e1.a - e1.c * e2.a ) / d;
			
			if ( (e1.reg[1].coord.y < e2.reg[1].coord.y)
			    || (e1.reg[1].coord.y == e2.reg[1].coord.y && e1.reg[1].coord.x < e2.reg[1].coord.x) )
			{
				el = el1;
				e = e1;
			}
			else
			{
				el = el2;
				e = e2;
			}
			
			right_of_site = xint >= e.reg[1].coord.x;
			if ((right_of_site && el.ELpm == LE)
			    || (!right_of_site && el.ELpm == RE))
				return null;
			
			// create a new site at the Vector2 of intersection - this is a new vector
			// event waiting to happen
			v = new Site();
			v.coord.x = xint;
			v.coord.y = yint;
			return v;
		}
		
		/*
		 * implicit parameters: nsites, sqrt_nsites, xmin, xmax, ymin, ymax, deltax,
		 * deltay (can all be estimates). Performance suffers if they are wrong;
		 * better to make nsites, deltax, and deltay too big than too small. (?)
		 */
		bool Voronoi_bd()
		{
			Site newsite, bot, top, temp, p;
			Site v;
			Vector2? newintstar = null;
			int pm;
			Halfedge lbnd, rbnd, llbnd, rrbnd, bisector;
			Edge e;

			PQ_Initialize();
			EL_Initialize();

			bottomsite = NextOne();
			newsite = NextOne();
			while (true)
			{
				if (!PQ_Empty())
				{
					newintstar = PQ_min();
				}
				// if the lowest site has a smaller y value than the lowest vector
				// intersection,
				// process the site otherwise process the vector intersection

				if (newsite != null && (PQ_Empty()
				                        || newsite.coord.y < newintstar.Value.y
				                        || (newsite.coord.y == newintstar.Value.y
				                            && newsite.coord.x < newintstar.Value.x)))
				{
					/* new site is smallest -this is a site event */
					// get the first HalfEdge to the LEFT of the new site
					lbnd = EL_LeftBound((newsite.coord));
                    // get the first HalfEdge to the RIGHT of the new site
                    rbnd = lbnd.ELright;
					// if this halfedge has no edge,bot =bottom site (whatever that
					// is)
					bot = RightReg(lbnd);
					// create a new edge that bisects
					e = Bisect(bot, newsite);

					// create a new HalfEdge, setting its ELpm field to 0
					bisector = CreateHalfEdge(e, LE);
					// insert this new bisector edge between the left and right
					// vectors in a linked list
					EL_Insert(lbnd, bisector);

					// if the new bisector intersects with the left edge,
					// remove the left edge's vertex, and put in the new one
					if ((p = Intersect(lbnd, bisector)) != null)
					{
						PQ_Delete(lbnd);
						PQ_Insert(lbnd, p, Distance(p, newsite));
					}
					lbnd = bisector;
					// create a new HalfEdge, setting its ELpm field to 1
					bisector = CreateHalfEdge(e, RE);
					// insert the new HE to the right of the original bisector
					// earlier in the IF stmt
					EL_Insert(lbnd, bisector);

					// if this new bisector intersects with the new HalfEdge
					if ((p = Intersect(bisector, rbnd)) != null)
					{
						// push the HE into the ordered linked list of vertices
						PQ_Insert(bisector, p, Distance(p, newsite));
					}
					newsite = NextOne();
				} else if (!PQ_Empty())
					/* intersection is smallest - this is a vector event */
				{
					// pop the HalfEdge with the lowest vector off the ordered list
					// of vectors
					lbnd = PQ_Extractmin();
					// get the HalfEdge to the left of the above HE
					llbnd = lbnd.ELleft;
					// get the HalfEdge to the right of the above HE
					rbnd = lbnd.ELright;
					// get the HalfEdge to the right of the HE to the right of the
					// lowest HE
					rrbnd = rbnd.ELright;
					// get the Site to the left of the left HE which it bisects
					bot = LeftReg(lbnd);
					// get the Site to the right of the right HE which it bisects
					top = RightReg(rbnd);

					v = lbnd.vertex; // get the vertex that caused this event
					MakeVertex(v); // set the vertex number - couldn't do this
					// earlier since we didn't know when it would be processed
					EndVector2(lbnd.ELedge, lbnd.ELpm, v);
					// set the endVector2 of
					// the left HalfEdge to be this vector
					EndVector2(rbnd.ELedge, rbnd.ELpm, v);
					// set the endVector2 of the right HalfEdge to
					// be this vector
					EL_Delete(lbnd); // mark the lowest HE for
					// deletion - can't delete yet because there might be Vector2ers
					// to it in Hash Map
					PQ_Delete(rbnd);
					// remove all vertex events to do with the right HE
					EL_Delete(rbnd); // mark the right HE for
					// deletion - can't delete yet because there might be Vector2ers
					// to it in Hash Map
					pm = LE; // set the pm variable to zero

					if (bot.coord.y > top.coord.y)
						// if the site to the left of the event is higher than the
						// Site
					{ // to the right of it, then swap them and set the 'pm'
						// variable to 1
						temp = bot;
						bot = top;
						top = temp;
						pm = RE;
					}
					e = Bisect(bot, top); // create an Edge (or line)
					// that is between the two Sites. This creates the formula of
					// the line, and assigns a line number to it
					bisector = CreateHalfEdge(e, pm); // create a HE from the Edge 'e',
					// and make it Vector2 to that edge
					// with its ELedge field
					EL_Insert(llbnd, bisector); // insert the new bisector to the
					// right of the left HE
					EndVector2(e, RE - pm, v); // set one endVector2 to the new edge
					// to be the vector Vector2 'v'.
					// If the site to the left of this bisector is higher than the
					// right Site, then this endVector2
					// is put in position 0; otherwise in pos 1

					// if left HE and the new bisector intersect, then delete
					// the left HE, and reinsert it
					if ((p = Intersect(llbnd, bisector)) != null)
					{
						PQ_Delete(llbnd);
						PQ_Insert(llbnd, p, Distance(p, bot));
					}

					// if right HE and the new bisector intersect, then
					// reinsert it
					if ((p = Intersect(bisector, rrbnd)) != null)
					{
						PQ_Insert(bisector, p, Distance(p, bot));
					}
				} else
				{
					break;
				}
			}

			for (lbnd = ELleftend.ELright; lbnd != ELrightend; lbnd = lbnd.ELright)
			{
				e = lbnd.ELedge;
				ClipLine(e);
			}

			return true;
		}

	}
}