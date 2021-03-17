using Clusterization._2DArray;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clusterization.Text
{
    public class BookCollection
    {
        private int _maxTermsCount = 3000;

        public List<Book> Books { get; set; }
        public HashSet<string> Vocabulary { get; set; }

        public BookCollection()
        {
            Books = new List<Book>();
            Vocabulary = new HashSet<string>();
        }

        public void LoadBooks(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            var fileNames = files.Select(file => file.Split('(')[0]).ToList();
            var fileTexts = new List<string>();

            foreach (var file in files)
            {
                var text = File.ReadAllText(file, Encoding.GetEncoding(1251));
                text = Regex.Replace(text.ToLower(), @"[^ а-я]", string.Empty);

                fileTexts.Add(text);
            }

            var fileTerms = fileTexts.Select(text => text.Split(' ').Where(term => term != "").Take(_maxTermsCount));

            GetVocabulary(fileTerms);

            var fileVectors = GetVectors(fileTerms);

            for (int i = 0; i < files.Length; i++)
            {
                Book book = new Book
                {
                    Title = fileNames[i],
                    Vector = fileVectors[i]
                };

                Books.Add(book);
            }
        }

        void GetVocabulary(IEnumerable<IEnumerable<string>> fileTerms)
        {
            foreach (var file in fileTerms)
            {
                foreach (var term in file)
                {
                    Vocabulary.Add(term);
                }
            }
        }

        List<Matrix> GetVectors (IEnumerable<IEnumerable<string>> fileTerms)
        {
            List<Matrix> fileVectors = new List<Matrix>();

            for (int i = 0; i < fileTerms.Count(); i++)
            {
                Matrix vector = new Matrix(1, Vocabulary.Count);

                for (int j = 0; j < Vocabulary.Count; j++)
                {
                    vector[j] = FindTFIDF(Vocabulary.ElementAt(j), fileTerms.ElementAt(i), fileTerms);
                }

                fileVectors.Add(vector);
            }

            return fileVectors;
        }

        private double FindTFIDF(string term, IEnumerable<string> terms, IEnumerable<IEnumerable<string>> fileTerms)
        {
            double tf = FindTermFrequency(term, terms);
            double idf = FindInverseDocumentFrequency(term, fileTerms);

            return tf * idf;
        }

        private double FindTermFrequency(string term, IEnumerable<string> terms)
        {
            int count = terms.Count(x => x == term);

            return (count / (double)terms.Count());
        }


        private double FindInverseDocumentFrequency(string term, IEnumerable<IEnumerable<string>> fileTerms)
        {
            int count = fileTerms.Count(x => x.Contains(term));

            return Math.Log(fileTerms.Count() / (double)count);
        }
    }
}
