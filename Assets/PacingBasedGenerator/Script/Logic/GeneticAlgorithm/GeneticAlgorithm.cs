using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PBGGeneticAlgorithm {
    public class GeneticAlgorithm {
        //private int _geneSize;
        private int _populationSize;
        private int _crossoverChance;
        private int _mutationChance;
        private int _numberOfGeneration;
        private List<Chromosome> _population;

        public GeneticAlgorithm(int populationSize, int crossOverChance, int mutationChance, int numberofGeneration) {
            this._populationSize = populationSize;
            this._crossoverChance = crossOverChance;
            this._mutationChance = mutationChance;
            this._numberOfGeneration = numberofGeneration;
            InitializePopulation();
        }
        public void InitializePopulation() {
            this._population = new List<Chromosome>();
            // [Edit Later] Loop for initialization
            // [Edit Later] Add genome to population
        }
        
        public void PreSelection() { }
        public void Selection() { }
        public void Evaluation() { }
        public void CrossOver() { }
        public void Mutation() { }
        public void NextGeneration() { }
        public Chromosome GetFittest() { return null; }
        /*
         GetFittest
         GetFitness
         Selection
         NextGeneration
         SpinRouletteWheel
         CrossOver
         Mutation
         */
    }
}
