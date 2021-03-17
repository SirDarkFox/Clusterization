using k_clustering.k_means.Objects;
using k_clustering.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace k_clustering.k_means
{
    public class DocumentVectorModel
    {
        const int maxTextLength = 3000;
        private Regex r = new Regex("([ \\t{}()\",:;. \n])");
        private bool porter;

        public List<DocumentVector> DocumentVectorCollection { get; set; }
        public HashSet<string> Vocabulary { get; set; }

        public DocumentVectorModel(bool porter)
        {
            this.porter = porter;

            DocumentVectorCollection = new List<DocumentVector>();
            Vocabulary = new HashSet<string>();
        }

        public void CreateDVCollection(List<string> textCollection, string[] names)
        {
            TransformTextCollection(textCollection);
            GetVocabulary();
            GetDVCollection(names);
        }

        public void TransformTextCollection(List<string> textCollection)
        {
            for (int i = 0; i < textCollection.Count; i++)
            {
                var document = new DocumentVector();
                document.Content = new List<string>();
                foreach (var term in r.Split(textCollection[i]))
                {
                    if (document.Content.Count >= maxTextLength)
                        break;

                    string temp = Regex.Replace(term.ToLower(), @"[^а-я]", string.Empty);

                    if (porter)
                        temp = Porter.TransformWord(temp);

                    if (temp != "")
                        document.Content.Add(temp);
                }
                DocumentVectorCollection.Add(document);
            }
        }

        public void GetVocabulary()
        {
            foreach (var document in DocumentVectorCollection)
            {
                foreach (var term in document.Content)
                {
                    Vocabulary.Add(term);
                }
            }
        }

        private void GetDVCollection(string[] names)
        {
            for (int i = 0; i < DocumentVectorCollection.Count; i++)
            {
                var vector = new double[Vocabulary.Count];
                for (int j = 0; j < Vocabulary.Count; j++)
                {
                    vector[j] = FindTFIDF(DocumentVectorCollection[i].Content, Vocabulary.ElementAt(j));
                }

                DocumentVectorCollection[i].Name = names[i];
                DocumentVectorCollection[i].Vector = vector;
            }
        }

        private double FindTFIDF(List<string> content, string term)
        {
            double tf = FindTermFrequency(content, term);
            double idf = FindInverseDocumentFrequency(term);

            return tf * idf;
        }

        private double FindTermFrequency(List<string> content, string term)
        {
            int count = content.Count(x => x == term);

            return (count / (double)content.Count());
        }


        private double FindInverseDocumentFrequency(string term)
        {
            int count = DocumentVectorCollection.Count(x => x.Content.Contains(term));

            return Math.Log(DocumentVectorCollection.Count / (double)count);

        }
    }
}

