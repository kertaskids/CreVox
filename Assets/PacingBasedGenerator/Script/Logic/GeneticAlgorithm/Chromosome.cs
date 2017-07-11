using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBGGeneticAlgorithm { 
    public class Chromosome {
        public enum FitnessType {
            Threat = 0,
            Impetus = 1,
            Tempo = 2
        }
        public struct Fitness {
            float Threat;
            float Impetus;
            float Tempo;
        }
        private Fitness _pacingAspect;
        public Fitness PacingAspect {
            get { return _pacingAspect; }
            set { _pacingAspect = value; }
        }

        private Guid _id;
        private Gene[] _genes;

        public Chromosome(int maxGenes) {
            _genes = new Gene[maxGenes];
            _id = Guid.NewGuid();
        }

        public Guid ID {
            get { return _id; }
            set { _id = value; }
        }

        public void SetGeneOnIndex(int index, Gene gene) {
            _genes[index] = gene;
        }
        // For Crossover
        public void SwapWith(Chromosome chromosome, int toPosition) { }
        // For Mutation
        public void SwapGenes(int position1, int position2) { }

        public float FitnessThreat() { return 0; }
        public float FitnessImpetus() { return 0; }
        public float FitnessTempo() { return 0; }

        public float GetFitness(FitnessType fitnessType) {
            switch (fitnessType) {
                case FitnessType.Threat:
                    return FitnessThreat();
                case FitnessType.Impetus:
                    return FitnessImpetus();
                case FitnessType.Tempo:
                    return FitnessTempo();
                default:
                    return 0;
            }
        }

    }
}