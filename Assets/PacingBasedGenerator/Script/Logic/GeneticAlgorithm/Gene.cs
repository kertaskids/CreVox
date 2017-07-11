using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBGGeneticAlgorithm {
    public class Gene {
        public enum GeneType {
            None,
            Enemy,
            Obstacle,
            Traps
        }
        public GeneType Type;
        // Info about tile, ie. transform

        public Gene() { }
        public Gene(GeneType type) {
            this.Type = type;
        }
        public void Copy(Gene sourceGene) {
            this.Type = sourceGene.Type;
        }
    }
}
