using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace seyana
{
    class SeyanaVoice
    {
        private static SeyanaVoice SeyanaVoiceIns = null;
        public static SeyanaVoice SeyanaVoiceFactory
        {
            get
            {
                if (SeyanaVoiceIns == null)
                {
                    SeyanaVoiceIns = new SeyanaVoice();
                    return SeyanaVoiceIns;
                }
                else return SeyanaVoiceIns;
            }
        }

        private Dictionary<situation, SoundPlayer> sps;

        public enum situation
        {
            SEYANA, YADE, EBIFRY
        }

        private void init(Dictionary<situation, string> lst)
        {
            sps = new Dictionary<situation, SoundPlayer>(lst.Count);
            foreach (var s in lst.Keys)
            {
                sps.Add(s, new SoundPlayer(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("seyana.se." + lst[s])));
                sps[s].Load();
            }
        }

        private SeyanaVoice() {
            var defaultDictionary = new Dictionary<situation, string>();
            defaultDictionary[situation.SEYANA] = "seyana_seyana.wav";
            defaultDictionary[situation.YADE] = "seyana_yade.wav";
            defaultDictionary[situation.EBIFRY] = "seyana_ebifry.wav";

            init(defaultDictionary);
            isPlaying = false;
        }

/*        public SeyanaVoice(Dictionary<situation, string> dict)
        {
            init(dict);
        } */

        private void playSE(situation s)
        {
            if (!isPlaying) Task.Factory.StartNew(() => playSE_run(s));
        }

        public static bool isPlaying { private set; get; }
        private void playSE_run(situation s)
        {
            isPlaying = true;
            sps[s].PlaySync();
            isPlaying = false;
        }

        public void playSeyana()
        {
            playSE(situation.SEYANA);
        }
        public void playYade()
        {
            playSE(situation.YADE);
        }
        public void playEbifry()
        {
            playSE(situation.EBIFRY);
        }
    }
}
