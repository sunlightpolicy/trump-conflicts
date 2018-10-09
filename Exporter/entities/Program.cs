using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using edu.stanford.nlp.parser;
using java.util;
using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.util;

namespace entities {

    class Program {
        public static CRFClassifier Classifier =
            CRFClassifier.getClassifierNoExceptions(
               "c:\\english.all.3class.distsim.crf.ser.gz");

        static void Main(string[] args) {

            //string line = null;

            //while ((line = Console.ReadLine()) != null) {
                try {
                    // Input as docNo, url, domain, contentlength, tokens
                    //string[] rows = line.Split('\t');


                    //string docNo = rows[0];
                    //string tokens = rows[4];
                    //string docText = rows[5];

                    string docText = "Currently one of the fastest growing luxury hotel companies in the world and with a portfolio of 14 award-winning hotels, Trump properties are known for their unrivaled standard of excellence, personalized service and prime locations worldwide. The rapidly expanding luxury brand is opening four hotels in 2016 alone in major cities such as Washington D.C., Rio de Janeiro, Vancouver and Baku.  \"As we continue to strategically grow the Trump Hotel Collection, we seek only the world's most desirable destinations. Bali is consistently voted as one of the world's best islands and is one of the best resort and residential destinations globally,\" said Donald Trump Jr., executive vice president of development and acquisitions of The Trump Organization. \"This is an exciting time for Trump Hotels and we are honored to announce our expansion into Asia and bring to market an extraordinary luxury property.\"  Following the recent visit of an MNC Group delegation to New York City, Donald J. Trump and Mr. Hary Tanoesoedibjo hosted the official signing ceremony on Friday, 14th August 2015, in which the Founder, Mr. Hary, who is also Group President and CEO of MNC Group, mentioned, \"I am personally very pleased to bring the Trump brand into Indonesia.The Trump family's personal involvement and global proven success, is set out to elevate Bali's expectations for 'luxury and excellence'.Alongside our experience as a world -class developer, we are eager to develop the largest and most integrated lifestyle resort destination in Bali.\"  Established in 1989 and proudly developed to become the leading investment company in Indonesia, MNC Group engages in financial investments and focuses on 3 strategic fields, such as Media, Financial Services and Property. Under the leadership of its Group President and CEO, Mr. Hary Tanoesoedibjo, MNC Group is expected to continue growing rapidly capitalizing on Indonesia's large population base and rising upper middle class income through vision, quality and speed.  Trump Hotels and MNC Group plan to deliver the most prestigious resort and residential offerings in Asia, known as one of the most popular resort destinations in the world. The world-class property will have an integrated lifestyle resort vision providing the largest suites and villas with expansive views along with the finest combination of an unprecedented proposition of incredible amenities and six-star services, bringing a new level of luxury, quality and entertainment to Bali, \"The Isle of Gods\".  Built atop a sheer cliff along a sweeping coastline, the development will offer breathtaking views of the Indian Ocean and Tanah Lot, the most popular tourist and cultural icon of Bali. Wrapped in years of cultural and natural beauty, the newly developed resort will offer an enchanting and unrivaled getaway from the current luxury hotels' offering in the market.  About TRUMP HOTEL COLLECTION™  Launched in October 2007, TRUMP HOTEL COLLECTION™ is the next generation of luxury hospitality – one that is raising the bar for top-tier travel experiences with a level of customized service unrivaled in the market today. Presided over by internationally renowned developer Donald J. Trump and his three grown children – Donald Jr., Ivanka and Eric – the prestigious portfolio includes the highly acclaimed Trump International Hotel & Tower® New York, Trump International Hotel & Tower® Chicago, Trump International Hotel™ Las Vegas, Trump International Hotel™ Waikiki Beach Walk®, Trump SoHo® New York, Trump Ocean Club® International Hotel & Tower Panama, Trump International Hotel & Tower Toronto®, Trump National Doral® Miami, and Trump International Golf Links & Hotel™, Ireland. Trump® International Hotel & Tower Baku, Trump® Hotel Rio de Janeiro, Trump International® Hotel & Tower Vancouver and Trump® International Hotel, Washington, D.C. are slated to open in 2016. Reservations can be made at www.TRUMPHOTELCOLLECTION.com or by calling (855) TRUMP-00 (878-6700). TRUMP HOTEL COLLECTION is headquartered at Trump Tower, 725 Fifth Avenue, New York, NY 10022. Connect with TRUMP HOTEL COLLECTION on its social media pages.  About MNC Group  Founded in 1989 by Mr. Hary Tanoesoedibjo, MNC Group is Indonesia's leading investment company, engaging in financial investments and also focusing on media, financial services and property. MNC Media, operated under MNC Group's subsidiary Global Mediacom, is the most integrated media company in Southeast Asia with a wide portfolio as follows: free-to-air TV stations, with a combined audience share of 40%; paid-TV platforms, with over 2.5 million subscribers and a 70% market share; and online businesses, offering the fastest high speed internet service up to 500 mbps. MNC Financial Services provides a complete range of consumer financial products and services in areas that include healthcare, wealth management, life insurance, mortgages, leasing services, brokerage, investment banking and asset management, both domestic and offshore. Playing a leading role in growing the overall group's financial business is MNC Bank, which aims to be \"the bank of the future\" by developing the most advanced systems in digital banking. MNC Land acquires, develops and manages commercial and residential properties in Southeast Asia. MNC Land has quickly grown into one of the largest property groups in Indonesia, working with major properties that include Lido Resort, a 3,000-ha site in Bogor, West Java; The Westin Convention and Resort Hotel in Nusa Dua Bali; and Bali Nirwana Resort, a 100-ha hotel and golf course in Tabanan, Bali. In the general property development, investment and management, MNC Land currently also owns and develops several office buildings, apartments and hotels in the prime areas of Jakarta and Surabaya, such as Park Hyatt Hotel in Jakarta and Oakwood Residences in Surabaya, as well as having a significant investment in Plaza Indonesia Jakarta, which includes the Grand Hyatt hotel and Keraton Luxury Collection Hotel. MNC Corporation also operates financial investments, which objective is purely for a financial gain, including toll roads, coalmines and coal terminals. Other than its business activities, MNC Group is also active in social related assistance through its CSR Programs known as Jalinan Kasih, focusing into helping the poor through significant resources with scholarships, food supplies, free medical care, renovation of public facilities, assistance caused by natural disasters and raising funds through its TV programs for charity purposes. Through vision, quality and speed MNC Group www.mncgroup.com has become the leading investment company in Indonesia.";

                    var classified = Classifier.classifyToCharacterOffsets(docText).toArray();

                    for (int i = 0; i < classified.Length; i++) {
                        Triple triple = (Triple)classified[i];

                        int second = Convert.ToInt32(triple.second().ToString());
                        int third = Convert.ToInt32(triple.third().ToString());

                        Console.WriteLine(triple.first().ToString() + '\t' + docText.Substring(second, third - second));
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            //}
        }
    }
}
