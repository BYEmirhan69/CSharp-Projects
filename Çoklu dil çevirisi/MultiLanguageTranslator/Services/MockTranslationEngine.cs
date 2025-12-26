using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLanguageTranslator.Services
{
    /// <summary>
    /// Mock çeviri motoru - Gerçek API kullanmadan dictionary tabanlı çeviri yapar.
    /// Mock translation engine - Performs dictionary-based translation without real API.
    /// </summary>
    public class MockTranslationEngine
    {
        // Türkçe -> İngilizce sözlük
        private readonly Dictionary<string, string> _turkishToEnglish = new(StringComparer.OrdinalIgnoreCase)
        {
            { "merhaba", "hello" },
            { "dünya", "world" },
            { "nasılsın", "how are you" },
            { "günaydın", "good morning" },
            { "iyi", "good" },
            { "kötü", "bad" },
            { "evet", "yes" },
            { "hayır", "no" },
            { "teşekkürler", "thank you" },
            { "lütfen", "please" },
            { "hoşgeldiniz", "welcome" },
            { "güle güle", "goodbye" },
            { "ben", "I" },
            { "sen", "you" },
            { "o", "he/she/it" },
            { "biz", "we" },
            { "onlar", "they" },
            { "ev", "house" },
            { "araba", "car" },
            { "kitap", "book" },
            { "kalem", "pen" },
            { "masa", "table" },
            { "sandalye", "chair" },
            { "bilgisayar", "computer" },
            { "telefon", "phone" },
            { "su", "water" },
            { "ekmek", "bread" },
            { "yemek", "food" },
            { "çay", "tea" },
            { "kahve", "coffee" },
            { "süt", "milk" },
            { "bugün", "today" },
            { "yarın", "tomorrow" },
            { "dün", "yesterday" },
            { "şimdi", "now" },
            { "sonra", "later" },
            { "önce", "before" },
            { "büyük", "big" },
            { "küçük", "small" },
            { "güzel", "beautiful" },
            { "çirkin", "ugly" },
            { "hızlı", "fast" },
            { "yavaş", "slow" },
            { "sıcak", "hot" },
            { "soğuk", "cold" },
            { "yeni", "new" },
            { "eski", "old" },
            { "aşk", "love" },
            { "sevgi", "affection" },
            { "mutluluk", "happiness" },
            { "program", "program" },
            { "uygulama", "application" },
            { "çeviri", "translation" },
            { "dil", "language" }
        };

        // Türkçe -> Almanca sözlük
        private readonly Dictionary<string, string> _turkishToGerman = new(StringComparer.OrdinalIgnoreCase)
        {
            { "merhaba", "hallo" },
            { "dünya", "welt" },
            { "nasılsın", "wie geht es dir" },
            { "günaydın", "guten morgen" },
            { "iyi", "gut" },
            { "kötü", "schlecht" },
            { "evet", "ja" },
            { "hayır", "nein" },
            { "teşekkürler", "danke" },
            { "lütfen", "bitte" },
            { "hoşgeldiniz", "willkommen" },
            { "güle güle", "auf wiedersehen" },
            { "ben", "ich" },
            { "sen", "du" },
            { "o", "er/sie/es" },
            { "biz", "wir" },
            { "onlar", "sie" },
            { "ev", "haus" },
            { "araba", "auto" },
            { "kitap", "buch" },
            { "kalem", "stift" },
            { "masa", "tisch" },
            { "sandalye", "stuhl" },
            { "bilgisayar", "computer" },
            { "telefon", "telefon" },
            { "su", "wasser" },
            { "ekmek", "brot" },
            { "yemek", "essen" },
            { "çay", "tee" },
            { "kahve", "kaffee" },
            { "süt", "milch" },
            { "bugün", "heute" },
            { "yarın", "morgen" },
            { "dün", "gestern" },
            { "şimdi", "jetzt" },
            { "sonra", "später" },
            { "önce", "vorher" },
            { "büyük", "groß" },
            { "küçük", "klein" },
            { "güzel", "schön" },
            { "çirkin", "hässlich" },
            { "hızlı", "schnell" },
            { "yavaş", "langsam" },
            { "sıcak", "heiß" },
            { "soğuk", "kalt" },
            { "yeni", "neu" },
            { "eski", "alt" },
            { "aşk", "liebe" },
            { "sevgi", "zuneigung" },
            { "mutluluk", "glück" },
            { "program", "programm" },
            { "uygulama", "anwendung" },
            { "çeviri", "übersetzung" },
            { "dil", "sprache" }
        };

        // Türkçe -> Fransızca sözlük
        private readonly Dictionary<string, string> _turkishToFrench = new(StringComparer.OrdinalIgnoreCase)
        {
            { "merhaba", "bonjour" },
            { "dünya", "monde" },
            { "nasılsın", "comment allez-vous" },
            { "günaydın", "bonjour" },
            { "iyi", "bon" },
            { "kötü", "mauvais" },
            { "evet", "oui" },
            { "hayır", "non" },
            { "teşekkürler", "merci" },
            { "lütfen", "s'il vous plaît" },
            { "hoşgeldiniz", "bienvenue" },
            { "güle güle", "au revoir" },
            { "ben", "je" },
            { "sen", "tu" },
            { "o", "il/elle" },
            { "biz", "nous" },
            { "onlar", "ils/elles" },
            { "ev", "maison" },
            { "araba", "voiture" },
            { "kitap", "livre" },
            { "kalem", "stylo" },
            { "masa", "table" },
            { "sandalye", "chaise" },
            { "bilgisayar", "ordinateur" },
            { "telefon", "téléphone" },
            { "su", "eau" },
            { "ekmek", "pain" },
            { "yemek", "nourriture" },
            { "çay", "thé" },
            { "kahve", "café" },
            { "süt", "lait" },
            { "bugün", "aujourd'hui" },
            { "yarın", "demain" },
            { "dün", "hier" },
            { "şimdi", "maintenant" },
            { "sonra", "après" },
            { "önce", "avant" },
            { "büyük", "grand" },
            { "küçük", "petit" },
            { "güzel", "beau" },
            { "çirkin", "laid" },
            { "hızlı", "rapide" },
            { "yavaş", "lent" },
            { "sıcak", "chaud" },
            { "soğuk", "froid" },
            { "yeni", "nouveau" },
            { "eski", "vieux" },
            { "aşk", "amour" },
            { "sevgi", "affection" },
            { "mutluluk", "bonheur" },
            { "program", "programme" },
            { "uygulama", "application" },
            { "çeviri", "traduction" },
            { "dil", "langue" }
        };

        // Türkçe -> İspanyolca sözlük
        private readonly Dictionary<string, string> _turkishToSpanish = new(StringComparer.OrdinalIgnoreCase)
        {
            { "merhaba", "hola" },
            { "dünya", "mundo" },
            { "nasılsın", "cómo estás" },
            { "günaydın", "buenos días" },
            { "iyi", "bueno" },
            { "kötü", "malo" },
            { "evet", "sí" },
            { "hayır", "no" },
            { "teşekkürler", "gracias" },
            { "lütfen", "por favor" },
            { "hoşgeldiniz", "bienvenido" },
            { "güle güle", "adiós" },
            { "ben", "yo" },
            { "sen", "tú" },
            { "o", "él/ella" },
            { "biz", "nosotros" },
            { "onlar", "ellos/ellas" },
            { "ev", "casa" },
            { "araba", "coche" },
            { "kitap", "libro" },
            { "kalem", "bolígrafo" },
            { "masa", "mesa" },
            { "sandalye", "silla" },
            { "bilgisayar", "computadora" },
            { "telefon", "teléfono" },
            { "su", "agua" },
            { "ekmek", "pan" },
            { "yemek", "comida" },
            { "çay", "té" },
            { "kahve", "café" },
            { "süt", "leche" },
            { "bugün", "hoy" },
            { "yarın", "mañana" },
            { "dün", "ayer" },
            { "şimdi", "ahora" },
            { "sonra", "después" },
            { "önce", "antes" },
            { "büyük", "grande" },
            { "küçük", "pequeño" },
            { "güzel", "hermoso" },
            { "çirkin", "feo" },
            { "hızlı", "rápido" },
            { "yavaş", "lento" },
            { "sıcak", "caliente" },
            { "soğuk", "frío" },
            { "yeni", "nuevo" },
            { "eski", "viejo" },
            { "aşk", "amor" },
            { "sevgi", "cariño" },
            { "mutluluk", "felicidad" },
            { "program", "programa" },
            { "uygulama", "aplicación" },
            { "çeviri", "traducción" },
            { "dil", "idioma" }
        };

        // İngilizce -> Türkçe sözlük (tersi)
        private readonly Dictionary<string, string> _englishToTurkish = new(StringComparer.OrdinalIgnoreCase)
        {
            { "hello", "merhaba" },
            { "world", "dünya" },
            { "how are you", "nasılsın" },
            { "good morning", "günaydın" },
            { "good", "iyi" },
            { "bad", "kötü" },
            { "yes", "evet" },
            { "no", "hayır" },
            { "thank you", "teşekkürler" },
            { "please", "lütfen" },
            { "welcome", "hoşgeldiniz" },
            { "goodbye", "güle güle" },
            { "I", "ben" },
            { "you", "sen" },
            { "he", "o" },
            { "she", "o" },
            { "it", "o" },
            { "we", "biz" },
            { "they", "onlar" },
            { "house", "ev" },
            { "car", "araba" },
            { "book", "kitap" },
            { "pen", "kalem" },
            { "table", "masa" },
            { "chair", "sandalye" },
            { "computer", "bilgisayar" },
            { "phone", "telefon" },
            { "water", "su" },
            { "bread", "ekmek" },
            { "food", "yemek" },
            { "tea", "çay" },
            { "coffee", "kahve" },
            { "milk", "süt" },
            { "today", "bugün" },
            { "tomorrow", "yarın" },
            { "yesterday", "dün" },
            { "now", "şimdi" },
            { "later", "sonra" },
            { "before", "önce" },
            { "big", "büyük" },
            { "small", "küçük" },
            { "beautiful", "güzel" },
            { "ugly", "çirkin" },
            { "fast", "hızlı" },
            { "slow", "yavaş" },
            { "hot", "sıcak" },
            { "cold", "soğuk" },
            { "new", "yeni" },
            { "old", "eski" },
            { "love", "aşk" },
            { "affection", "sevgi" },
            { "happiness", "mutluluk" },
            { "program", "program" },
            { "application", "uygulama" },
            { "translation", "çeviri" },
            { "language", "dil" }
        };

        // İngilizce -> Almanca sözlük
        private readonly Dictionary<string, string> _englishToGerman = new(StringComparer.OrdinalIgnoreCase)
        {
            { "hello", "hallo" },
            { "world", "welt" },
            { "how are you", "wie geht es dir" },
            { "good morning", "guten morgen" },
            { "good", "gut" },
            { "bad", "schlecht" },
            { "yes", "ja" },
            { "no", "nein" },
            { "thank you", "danke" },
            { "please", "bitte" },
            { "welcome", "willkommen" },
            { "goodbye", "auf wiedersehen" },
            { "I", "ich" },
            { "you", "du" },
            { "he", "er" },
            { "she", "sie" },
            { "it", "es" },
            { "we", "wir" },
            { "they", "sie" },
            { "house", "haus" },
            { "car", "auto" },
            { "book", "buch" },
            { "pen", "stift" },
            { "table", "tisch" },
            { "chair", "stuhl" },
            { "computer", "computer" },
            { "phone", "telefon" },
            { "water", "wasser" },
            { "bread", "brot" },
            { "food", "essen" },
            { "tea", "tee" },
            { "coffee", "kaffee" },
            { "milk", "milch" },
            { "love", "liebe" },
            { "program", "programm" },
            { "application", "anwendung" },
            { "translation", "übersetzung" },
            { "language", "sprache" }
        };

        // İngilizce -> Fransızca sözlük
        private readonly Dictionary<string, string> _englishToFrench = new(StringComparer.OrdinalIgnoreCase)
        {
            { "hello", "bonjour" },
            { "world", "monde" },
            { "how are you", "comment allez-vous" },
            { "good morning", "bonjour" },
            { "good", "bon" },
            { "bad", "mauvais" },
            { "yes", "oui" },
            { "no", "non" },
            { "thank you", "merci" },
            { "please", "s'il vous plaît" },
            { "welcome", "bienvenue" },
            { "goodbye", "au revoir" },
            { "I", "je" },
            { "you", "tu" },
            { "he", "il" },
            { "she", "elle" },
            { "it", "ce" },
            { "we", "nous" },
            { "they", "ils" },
            { "house", "maison" },
            { "car", "voiture" },
            { "book", "livre" },
            { "pen", "stylo" },
            { "table", "table" },
            { "chair", "chaise" },
            { "computer", "ordinateur" },
            { "phone", "téléphone" },
            { "water", "eau" },
            { "bread", "pain" },
            { "food", "nourriture" },
            { "tea", "thé" },
            { "coffee", "café" },
            { "milk", "lait" },
            { "love", "amour" },
            { "program", "programme" },
            { "application", "application" },
            { "translation", "traduction" },
            { "language", "langue" }
        };

        // İngilizce -> İspanyolca sözlük
        private readonly Dictionary<string, string> _englishToSpanish = new(StringComparer.OrdinalIgnoreCase)
        {
            { "hello", "hola" },
            { "world", "mundo" },
            { "how are you", "cómo estás" },
            { "good morning", "buenos días" },
            { "good", "bueno" },
            { "bad", "malo" },
            { "yes", "sí" },
            { "no", "no" },
            { "thank you", "gracias" },
            { "please", "por favor" },
            { "welcome", "bienvenido" },
            { "goodbye", "adiós" },
            { "I", "yo" },
            { "you", "tú" },
            { "he", "él" },
            { "she", "ella" },
            { "it", "eso" },
            { "we", "nosotros" },
            { "they", "ellos" },
            { "house", "casa" },
            { "car", "coche" },
            { "book", "libro" },
            { "pen", "bolígrafo" },
            { "table", "mesa" },
            { "chair", "silla" },
            { "computer", "computadora" },
            { "phone", "teléfono" },
            { "water", "agua" },
            { "bread", "pan" },
            { "food", "comida" },
            { "tea", "té" },
            { "coffee", "café" },
            { "milk", "leche" },
            { "love", "amor" },
            { "program", "programa" },
            { "application", "aplicación" },
            { "translation", "traducción" },
            { "language", "idioma" }
        };

        /// <summary>
        /// Desteklenen kaynak dilleri döndürür.
        /// Returns supported source languages.
        /// </summary>
        public List<string> GetSourceLanguages()
        {
            return new List<string> { "Türkçe", "İngilizce" };
        }

        /// <summary>
        /// Desteklenen hedef dilleri döndürür.
        /// Returns supported target languages.
        /// </summary>
        public List<string> GetTargetLanguages()
        {
            return new List<string> { "İngilizce", "Almanca", "Fransızca", "İspanyolca", "Türkçe" };
        }

        /// <summary>
        /// Metni belirtilen kaynak dilden hedef dile çevirir.
        /// Translates text from specified source language to target language.
        /// </summary>
        /// <param name="text">Çevrilecek metin / Text to translate</param>
        /// <param name="sourceLanguage">Kaynak dil / Source language</param>
        /// <param name="targetLanguage">Hedef dil / Target language</param>
        /// <returns>Çevrilmiş metin / Translated text</returns>
        public string Translate(string text, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Aynı dil kontrolü
            if (sourceLanguage == targetLanguage)
                return text;

            Dictionary<string, string>? dictionary = GetDictionary(sourceLanguage, targetLanguage);

            if (dictionary == null)
            {
                // Sözlük bulunamadı, basit dönüşüm yap
                return TransformText(text, targetLanguage);
            }

            return TranslateWithDictionary(text, dictionary, targetLanguage);
        }

        /// <summary>
        /// Uygun sözlüğü seçer.
        /// Selects the appropriate dictionary.
        /// </summary>
        private Dictionary<string, string>? GetDictionary(string sourceLanguage, string targetLanguage)
        {
            // Türkçe'den çeviri
            if (sourceLanguage == "Türkçe")
            {
                return targetLanguage switch
                {
                    "İngilizce" => _turkishToEnglish,
                    "Almanca" => _turkishToGerman,
                    "Fransızca" => _turkishToFrench,
                    "İspanyolca" => _turkishToSpanish,
                    _ => null
                };
            }
            // İngilizce'den çeviri
            else if (sourceLanguage == "İngilizce")
            {
                return targetLanguage switch
                {
                    "Türkçe" => _englishToTurkish,
                    "Almanca" => _englishToGerman,
                    "Fransızca" => _englishToFrench,
                    "İspanyolca" => _englishToSpanish,
                    _ => null
                };
            }

            return null;
        }

        /// <summary>
        /// Sözlük kullanarak çeviri yapar.
        /// Performs translation using dictionary.
        /// </summary>
        private string TranslateWithDictionary(string text, Dictionary<string, string> dictionary, string targetLanguage)
        {
            StringBuilder result = new StringBuilder();
            string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                string cleanWord = CleanWord(word);
                string punctuation = GetPunctuation(word);

                if (dictionary.TryGetValue(cleanWord, out string? translated))
                {
                    // Büyük harf kontrolü
                    if (char.IsUpper(cleanWord[0]) && translated.Length > 0)
                    {
                        translated = char.ToUpper(translated[0]) + translated.Substring(1);
                    }
                    result.Append(translated);
                }
                else
                {
                    // Çeviri bulunamadı, kelimeyi hedef dil formatında göster
                    result.Append($"[{cleanWord}]");
                }

                result.Append(punctuation);

                if (i < words.Length - 1)
                {
                    result.Append(" ");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Kelimeden noktalama işaretlerini temizler.
        /// Cleans punctuation from word.
        /// </summary>
        private string CleanWord(string word)
        {
            StringBuilder clean = new StringBuilder();
            foreach (char c in word)
            {
                if (char.IsLetter(c))
                {
                    clean.Append(c);
                }
            }
            return clean.ToString();
        }

        /// <summary>
        /// Kelimenin sonundaki noktalama işaretlerini alır.
        /// Gets punctuation at the end of word.
        /// </summary>
        private string GetPunctuation(string word)
        {
            StringBuilder punct = new StringBuilder();
            for (int i = word.Length - 1; i >= 0; i--)
            {
                if (char.IsPunctuation(word[i]))
                {
                    punct.Insert(0, word[i]);
                }
                else
                {
                    break;
                }
            }
            return punct.ToString();
        }

        /// <summary>
        /// Sözlük bulunamadığında basit dönüşüm yapar.
        /// Performs simple transformation when dictionary not found.
        /// </summary>
        private string TransformText(string text, string targetLanguage)
        {
            // Hedef dile göre basit ön ek ekle
            string prefix = targetLanguage switch
            {
                "İngilizce" => "[EN] ",
                "Almanca" => "[DE] ",
                "Fransızca" => "[FR] ",
                "İspanyolca" => "[ES] ",
                "Türkçe" => "[TR] ",
                _ => "[??] "
            };

            return $"{prefix}{text}";
        }

        /// <summary>
        /// Birden fazla hedef dile çeviri yapar.
        /// Translates to multiple target languages.
        /// </summary>
        /// <param name="text">Çevrilecek metin / Text to translate</param>
        /// <param name="sourceLanguage">Kaynak dil / Source language</param>
        /// <param name="targetLanguages">Hedef diller / Target languages</param>
        /// <returns>Dil-Çeviri sözlüğü / Language-Translation dictionary</returns>
        public Dictionary<string, string> TranslateToMultiple(string text, string sourceLanguage, List<string> targetLanguages)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            foreach (string targetLanguage in targetLanguages)
            {
                string translation = Translate(text, sourceLanguage, targetLanguage);
                results[targetLanguage] = translation;
            }

            return results;
        }
    }
}
