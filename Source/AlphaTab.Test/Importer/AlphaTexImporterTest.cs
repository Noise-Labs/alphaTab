﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AlphaTab.Collections;
using AlphaTab.Importer;
using AlphaTab.IO;
using AlphaTab.Model;
using AlphaTab.Rendering;
using AlphaTab.Rendering.Effects;
using AlphaTab.Rendering.Glyphs;
using AlphaTab.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlphaTab.Test.Importer
{
    [TestClass]
    public class AlphaTexImporterTest
    {
        private Score ParseTex(string tex)
        {
            var importer = new AlphaTexImporter();
            importer.Init(TestPlatform.CreateStringReader(tex));
            return importer.ReadScore();
        }

        [TestMethod]
        public void EnsureMetadataParsing_Issue73()
        {
            var tex = @"\title Test
                        \words test
                        \music alphaTab
                        \copyright test
                        \tempo 200
                        \instrument 30
                        \capo 2
                        \tuning G3 D2 G2 B2 D3 A4
                        .
                        0.5.2 1.5.4 3.4.4 | 5.3.8 5.3.8 5.3.8 5.3.8 r.2";

            var score = ParseTex(tex);

            Assert.AreEqual("Test", score.Title);
            Assert.AreEqual("test", score.Words);
            Assert.AreEqual("alphaTab", score.Music);
            Assert.AreEqual("test", score.Copyright);
            Assert.AreEqual(200, score.Tempo);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(30, score.Tracks[0].PlaybackInfo.Program);
            Assert.AreEqual(2, score.Tracks[0].Staves[0].Capo);
            Assert.AreEqual("55,38,43,47,50,69", string.Join(",", score.Tracks[0].Staves[0].Tuning));

            Assert.AreEqual(2, score.MasterBars.Count);

            Assert.AreEqual(3, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            {
                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes.Count);
                Assert.AreEqual(Duration.Half, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Duration);
                Assert.AreEqual(0, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].Fret);
                Assert.AreEqual(2, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].String);

                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes.Count);
                Assert.AreEqual(Duration.Quarter, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Duration);
                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes[0].Fret);
                Assert.AreEqual(2, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes[0].String);

                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes.Count);
                Assert.AreEqual(Duration.Quarter, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Duration);
                Assert.AreEqual(3, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes[0].Fret);
                Assert.AreEqual(3, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes[0].String);
            }

            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats.Count);
            {
                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes.Count);
                Assert.AreEqual(Duration.Eighth, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Duration);
                Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].Fret);
                Assert.AreEqual(4, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].String);

                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[1].Notes.Count);
                Assert.AreEqual(Duration.Eighth, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[1].Duration);
                Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[1].Notes[0].Fret);
                Assert.AreEqual(4, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[1].Notes[0].String);

                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[2].Notes.Count);
                Assert.AreEqual(Duration.Eighth, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[2].Duration);
                Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[2].Notes[0].Fret);
                Assert.AreEqual(4, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[2].Notes[0].String);

                Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[3].Notes.Count);
                Assert.AreEqual(Duration.Eighth, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[3].Duration);
                Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[3].Notes[0].Fret);
                Assert.AreEqual(4, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[3].Notes[0].String);

                Assert.AreEqual(0, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[4].Notes.Count);
                Assert.AreEqual(Duration.Half, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[4].Duration);
                Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[4].IsRest);
            }

        }


        [TestMethod]
        public void TestTuning()
        {
            var tex = @"\tuning E4 B3 G3 D3 A2 E2
                        .
                        0.5.1";

            var score = ParseTex(tex);

            Assert.AreEqual(string.Join(",", Tuning.GetDefaultTuningFor(6).Tunings), string.Join(",", score.Tracks[0].Staves[0].Tuning));
        }

        [TestMethod]
        public void DeadNotes1_Issue79()
        {
            var tex = @":4 x.3";

            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(0, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].Fret);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsDead);
        }

        [TestMethod]
        public void DeadNotes2_Issue79()
        {
            var tex = @":4 3.3{x}";

            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(0, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].Fret);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsDead);
        }

        [TestMethod]
        public void Trill_Issue79()
        {
            var tex = @":4 3.3{tr 5 16}";

            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(3, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].Fret);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsTrill);
            Assert.AreEqual(Duration.Sixteenth, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].TrillSpeed);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].TrillFret);
        }

        [TestMethod]
        public void Tremolo_Issue79()
        {
            var tex = @":4 3.3{tr 5 16}";

            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(3, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].Fret);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsTrill);
            Assert.AreEqual(Duration.Sixteenth, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].TrillSpeed);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].TrillFret);
        }

        [TestMethod]
        public void TremoloPicking_Issue79()
        {
            var tex = @":4 3.3{tp 16}";

            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(3, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].Fret);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].IsTremolo);
            Assert.AreEqual(Duration.Sixteenth, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].TremoloSpeed.Value);
        }

        [TestMethod]
        public void Hamonics_Issue79()
        {
            var tex = @":8 3.3{nh} 3.3{ah} 3.3{th} 3.3{ph} 3.3{sh}";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(HarmonicType.Natural, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].HarmonicType);
            Assert.AreEqual(HarmonicType.Artificial, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes[0].HarmonicType);
            Assert.AreEqual(HarmonicType.Tap, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes[0].HarmonicType);
            Assert.AreEqual(HarmonicType.Pinch, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[3].Notes[0].HarmonicType);
            Assert.AreEqual(HarmonicType.Semi, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[4].Notes[0].HarmonicType);
        }

        [TestMethod]
        public void HamonicsRenderingText_Issue79()
        {
            var tex = @":8 3.3{nh} 3.3{ah} 3.3{th} 3.3{ph} 3.3{sh}";
            var score = ParseTex(tex);

            Environment.StaveProfiles["harmonics"] = new BarRendererFactory[]
            {
                new EffectBarRendererFactory("n-harmonics", new IEffectBarRendererInfo[] {new HarmonicsEffectInfo(HarmonicType.Natural)}),
                new EffectBarRendererFactory("a-harmonics", new IEffectBarRendererInfo[] {new HarmonicsEffectInfo(HarmonicType.Artificial)}),
                new EffectBarRendererFactory("t-harmonics", new IEffectBarRendererInfo[] {new HarmonicsEffectInfo(HarmonicType.Tap)}),
                new EffectBarRendererFactory("p-harmonics", new IEffectBarRendererInfo[] {new HarmonicsEffectInfo(HarmonicType.Pinch)}),
                new EffectBarRendererFactory("s-harmonics", new IEffectBarRendererInfo[] {new HarmonicsEffectInfo(HarmonicType.Semi)}),
                new EffectBarRendererFactory("f-harmonics", new IEffectBarRendererInfo[] {new HarmonicsEffectInfo(HarmonicType.Feedback)}),
            };


            var settings = Settings.Defaults;
            settings.Engine = "svg";
            settings.Staves = new StaveSettings("harmonics");

            var renderer = new ScoreRenderer(settings);
            var svg = "";
            renderer.PartialRenderFinished += r =>
            {
                svg += r.RenderResult.ToString();
            };
            renderer.Render(score, new[] { 0 });

            var regexTemplate = @"<text[^>]+>\s*{0}\s*</text>";

            Assert.IsTrue(TestPlatform.IsMatch(svg, string.Format(regexTemplate, HarmonicsEffectInfo.HarmonicToString(HarmonicType.Natural))));
            Assert.IsTrue(TestPlatform.IsMatch(svg, string.Format(regexTemplate, HarmonicsEffectInfo.HarmonicToString(HarmonicType.Artificial))));
            Assert.IsTrue(TestPlatform.IsMatch(svg, string.Format(regexTemplate, HarmonicsEffectInfo.HarmonicToString(HarmonicType.Tap))));
            Assert.IsTrue(TestPlatform.IsMatch(svg, string.Format(regexTemplate, HarmonicsEffectInfo.HarmonicToString(HarmonicType.Pinch))));
            Assert.IsTrue(TestPlatform.IsMatch(svg, string.Format(regexTemplate, HarmonicsEffectInfo.HarmonicToString(HarmonicType.Semi))));
        }

        [TestMethod]
        public void Grace_Issue79()
        {
            var tex = @":8 3.3{gr} 3.3{gr ob}";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(2, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(GraceType.BeforeBeat, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].GraceType);
            Assert.AreEqual(GraceType.OnBeat, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].GraceType);
        }


        [TestMethod]
        public void BendRendering_Issue79()
        {
            var tex = @":4 15.6{b(0 4)} 18.6{b(0 6)} 17.6{b(0 8)} 16.6{b(0 3 0)} | 15.6{b(0 8 4)} 14.6{b(4 4)} 13.6{b(4 6)} 14.6{b(4 0)}";
            var score = ParseTex(tex);

            Environment.StaveProfiles["tabOnly"] = new BarRendererFactory[]
            {
                new TabBarRendererFactory(false, false, false),
            };

            var settings = Settings.Defaults;
            settings.Engine = "svg";
            settings.Layout.Mode = "horizontal";
            settings.Staves = new StaveSettings("tabOnly");

            var renderer = new ScoreRenderer(settings);
            var partials = new FastList<string>();
            renderer.Error += (o, e) =>
            {
                Assert.Fail(e.Message);
            };
            renderer.PartialRenderFinished += r =>
            {
                partials.Add(r.RenderResult.ToString());
            };
            renderer.Render(score, new[] { 0 });

            var tab = new XmlDocument(partials[0]);

            var texts = tab.GetElementsByTagName("text", true);

            var expectedTexts = new[]
            {
                Platform.Platform.StringFromCharCode((int)MusicFontSymbol.ClefTab), // clef

                "1", // bar number

                "15", "full",
                "18", "1½",
                "17", "2",
                "16", "¾",

                "2", // bar number

                "15", "2", "full",
                "14", "full",
                "13", "full", "1½",
                "14", "full"
            };

            for (int i = 0; i < expectedTexts.Length; i++)
            {
                var text = texts[i].InnerText.Trim();
                Assert.AreEqual(expectedTexts[i], text, "Mismatch at index {0}", i);
            }
        }

        [TestMethod]
        public void TestLeftHandFingerSingleNote()
        {
            var tex = @":8 3.3{lf 1} 3.3{lf 2} 3.3{lf 3} 3.3{lf 4} 3.3{lf 5}";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(Fingers.Thumb, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].LeftHandFinger);
            Assert.AreEqual(Fingers.IndexFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes[0].LeftHandFinger);
            Assert.AreEqual(Fingers.MiddleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes[0].LeftHandFinger);
            Assert.AreEqual(Fingers.AnnularFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[3].Notes[0].LeftHandFinger);
            Assert.AreEqual(Fingers.LittleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[4].Notes[0].LeftHandFinger);
        }

        [TestMethod]
        public void TestRightHandFingerSingleNote()
        {
            var tex = @":8 3.3{rf 1} 3.3{rf 2} 3.3{rf 3} 3.3{rf 4} 3.3{rf 5}";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(Fingers.Thumb, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].RightHandFinger);
            Assert.AreEqual(Fingers.IndexFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes[0].RightHandFinger);
            Assert.AreEqual(Fingers.MiddleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes[0].RightHandFinger);
            Assert.AreEqual(Fingers.AnnularFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[3].Notes[0].RightHandFinger);
            Assert.AreEqual(Fingers.LittleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[4].Notes[0].RightHandFinger);
        }

        [TestMethod]
        public void TestLeftHandFingerChord()
        {
            var tex = @":8 (3.1{lf 1} 3.2{lf 2} 3.3{lf 3} 3.4{lf 4} 3.5{lf 5})";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes.Count);
            Assert.AreEqual(Fingers.Thumb, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].LeftHandFinger);
            Assert.AreEqual(Fingers.IndexFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[1].LeftHandFinger);
            Assert.AreEqual(Fingers.MiddleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[2].LeftHandFinger);
            Assert.AreEqual(Fingers.AnnularFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[3].LeftHandFinger);
            Assert.AreEqual(Fingers.LittleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[4].LeftHandFinger);
        }

        [TestMethod]
        public void TestRightHandFingerChord()
        {
            var tex = @":8 (3.1{rf 1} 3.2{rf 2} 3.3{rf 3} 3.4{rf 4} 3.5{rf 5})";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(1, score.MasterBars.Count);
            Assert.AreEqual(1, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(5, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes.Count);
            Assert.AreEqual(Fingers.Thumb, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].RightHandFinger);
            Assert.AreEqual(Fingers.IndexFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[1].RightHandFinger);
            Assert.AreEqual(Fingers.MiddleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[2].RightHandFinger);
            Assert.AreEqual(Fingers.AnnularFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[3].RightHandFinger);
            Assert.AreEqual(Fingers.LittleFinger, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[4].RightHandFinger);
        }


        [TestMethod]
        public void TestUnstringed()
        {
            var tex = @"\tuning piano . c4 c#4 d4 d#4 | c4 db4 d4 eb4";
            var score = ParseTex(tex);

            Assert.AreEqual(1, score.Tracks.Count);
            Assert.AreEqual(2, score.MasterBars.Count);

            Assert.AreEqual(4, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats.Count);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(60, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].RealValue);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(61, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[1].Notes[0].RealValue);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(62, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[2].Notes[0].RealValue);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(63, score.Tracks[0].Staves[0].Bars[0].Voices[0].Beats[3].Notes[0].RealValue);


            Assert.AreEqual(4, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats.Count);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(60, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].RealValue);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(61, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[1].Notes[0].RealValue);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(62, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[2].Notes[0].RealValue);
            Assert.AreEqual(true, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[0].Notes[0].IsPiano);
            Assert.AreEqual(63, score.Tracks[0].Staves[0].Bars[1].Voices[0].Beats[3].Notes[0].RealValue);
        }
    }
}
