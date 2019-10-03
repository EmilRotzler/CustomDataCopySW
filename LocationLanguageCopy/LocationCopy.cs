using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationLanguageCopy.SmartwebApi;

namespace LocationLanguageCopy
{
    class LocationCopy
    {
        private WebServicePortClient client = new WebServicePortClient();
        private int CustomDataTypeId = 191; // here is the Custom data id that gets copied to different languages

        static void Main(string[] args)
        {
            LocationCopy locationCopy = new LocationCopy();
            Console.WriteLine("Starter...");

            locationCopy.CustomDataCopier();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
        
        public void CustomDataCopier()
        {
            //Connect
            client.Solution_Connect(Creds.swuser, Creds.swuser);

            //Lister der bliver brugt
            List<Product> products = new List<Product>();
            List<Product> productList = new List<Product>();
            List<ProductCustomData> productChecked = new List<ProductCustomData>();
            List<ProductCustomData> prodDataList = new List<ProductCustomData>();
            List<ProductCustomData> customdataList = new List<ProductCustomData>();

            //De data vi gerne vil have et produkt indeholder.
            client.Product_SetFields("CustomData, Id");
            //Få alle de forskellige sprog ind i en liste.

            customdataList = client.Product_GetCustomDataAll().ToList();
            products = client.Product_GetAll().ToList();
            productList.AddRange(products);

            List<Language> languages = new List<Language>();
            languages = Language.GetLanguages();

            //SolutionLanguage[] languages = client.Solution_GetLanguages();

            Console.WriteLine("Indsætter...");
            string title = string.Empty;
            foreach (Product product in productList)
            {
                //Tilføj produkternes CustomData til prodDataListe
                prodDataList.AddRange(product.CustomData);

                //Kører alle items i prodDataList igennem og indsætter den titel der skal bruges
                foreach (ProductCustomData prodCustomData in prodDataList)
                {
                if(prodCustomData.ProductCustomTypeId == 191) { 
                    //Hvis string title = null, vil der tjekkes om det nuværende CustomData's sproglag er sat til DK
                    //Hvis ja, vil string title blive sat til CustomData Title.
                    if (string.IsNullOrEmpty(title))
                    {
                        if (prodCustomData.LanguageISO == "DK")
                        {
                            title = prodCustomData.Title;
                        }
                    }
                        foreach (var language in languages)
                        {
                            if (language.LanguageISO != "DK")
                            {
                                client.Solution_SetLanguage(language.LanguageISO);
                                //Opdater ekstrafelt tekst ved det nuværende produkt til den titel som produktet har med sproglaget DK
                                client.Product_UpdateCustomTextData(product.Id, CustomDataTypeId, title);
                            }
                        }
                    }
                }
            }
        }
    }
}
