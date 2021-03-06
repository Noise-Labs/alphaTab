﻿/*
 * This file is part of alphaTab.
 * Copyright © 2018, Daniel Kuschny and Contributors, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or at your option any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */
using AlphaTab.Model;
using AlphaTab.Rendering.Glyphs;
using AlphaTab.Rendering.Staves;

namespace AlphaTab.Rendering
{
    class ScoreBeatContainerGlyph : BeatContainerGlyph
    {
        private ScoreBendGlyph _bend;

        public ScoreBeatContainerGlyph(Beat beat, VoiceContainerGlyph voiceContainer) : base(beat, voiceContainer)
        {
        }

        public override void DoLayout()
        {
            base.DoLayout();
            if (Beat.IsLegatoOrigin)
            {
                // only create slur for very first origin of "group"
                if (Beat.PreviousBeat == null || !Beat.PreviousBeat.IsLegatoOrigin)
                {
                    // tie with end beat
                    Beat destination = Beat.NextBeat;
                    while (destination.NextBeat != null && destination.NextBeat.IsLegatoDestination)
                    {
                        destination = destination.NextBeat;
                    }
                    Ties.Add(new ScoreLegatoGlyph(Beat, destination));
                }
            }
            else if (Beat.IsLegatoDestination)
            {
                // only create slur for last destination of "group"
                if (!Beat.IsLegatoOrigin)
                {
                    Beat origin = Beat.PreviousBeat;
                    while (origin.PreviousBeat != null && origin.PreviousBeat.IsLegatoOrigin)
                    {
                        origin = origin.PreviousBeat;
                    }
                    Ties.Add(new ScoreLegatoGlyph(origin, Beat, true));
                }
            }
            if (_bend != null)
            {
                _bend.Renderer = Renderer;
                _bend.DoLayout();
                UpdateWidth();
            }
        }

        protected override void CreateTies(Note n)
        {
            // create a tie if any effect requires it
            if (!n.IsVisible) return;

            // NOTE: we create 2 tie glyphs if we have a line break inbetween 
            // the two notes
            if (n.IsTieOrigin && !n.HasBend && !n.Beat.HasWhammyBar && n.Beat.GraceType != GraceType.BendGrace && n.TieDestination.IsVisible)
            {
                var tie = new ScoreTieGlyph(n, n.TieDestination);
                Ties.Add(tie);
            }

            if (n.IsTieDestination && !n.TieOrigin.HasBend && !n.Beat.HasWhammyBar)
            {
                var tie = new ScoreTieGlyph(n.TieOrigin, n, true);
                Ties.Add(tie);
            }

            // TODO: depending on the type we have other positioning
            // we should place glyphs in the preNotesGlyph or postNotesGlyph if needed
            if (n.SlideType != SlideType.None)
            {
                var l = new ScoreSlideLineGlyph(n.SlideType, n, this);
                Ties.Add(l);
            }

            if (n.Beat.SlurOrigin != null && n.Index == 0)
            {
                var tie = new ScoreSlurGlyph(n.Beat);
                Ties.Add(tie);
            }

            if (n.HasBend)
            {
                if (_bend == null)
                {
                    _bend = new ScoreBendGlyph(n.Beat);
                    _bend.Renderer = Renderer;
                    Ties.Add(_bend);
                }
                _bend.AddBends(n);
            }
        }
    }
}
