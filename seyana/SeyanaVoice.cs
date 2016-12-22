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
        private Dictionary<situation, SoundPlayer> sps;

        public enum situation
        {
            SEYANA
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

        public SeyanaVoice() {
            var defaultDictionary = new Dictionary<situation, string>();
            defaultDictionary[situation.SEYANA] = "seyana_seyana.wav";

            init(defaultDictionary);
        }
        public SeyanaVoice(Dictionary<situation, string> dict)
        {
            init(dict);
        }

        private void playSE(situation s)
        {
            sps[s].Play();
        }

        public void playSeyana()
        {
            playSE(situation.SEYANA);
        }
    }
}
