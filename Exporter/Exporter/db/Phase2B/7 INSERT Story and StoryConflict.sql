USE Trump
GO

INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'New York Times'), 2, 'https://www.nytimes.com/2018/02/28/business/jared-kushner-apollo-citigroup-loans.html', '', '2/28/2018 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'NPR'), 2, 'https://www.npr.org/2018/03/01/590022599/how-kushners-finances-could-be-potential-conflicts-of-interest', '', '3/1/2018 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'Trump, Inc. (WNYC)'), 2, 'https://www.wnycstudios.org/story/trump-inc-podcast-son-in-law-inc-secretive-real-estate-scion-white-house', '', '3/6/2018 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'Florida Division of Corporations'), 2, 'http://search.sunbiz.org/Inquiry/CorporationSearch/SearchResultDetail?inquirytype=EntityName&amp;directionType=Initial&amp;searchNameOrder=BUSYBOYSINVESTMENTS%20M050000006650&amp;aggregateId=forl-m05000000665-ae7bc44d-b6f6-45e4-b04d-86eba0d0f1b4&amp;searchTerm=busy%20boys%20investments&amp;listNameOrder=BUSYBOYSINVESTMENTS%20M050000006650', '', '6/28/2017 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'PR Newsire'), 2, 'http://www.prnewswire.com/news-releases/donald-trump-jr-joins-cambridge-whos-who-as-executive-director-of-global-branding-and-networking-90793174.html', '', '4/19/2010 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'New York Times'), 2, 'https://www.nytimes.com/2016/06/26/us/politics/cambridge-whos-who-trump-brand.html', '', '6/26/2016 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'The Post and Courier'), 2, 'http://www.postandcourier.com/business/titan-falls-a-second-time/article_0f46cf83-13f8-5f96-bc1e-8e97b8f43290.html', '', '4/11/2015 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'Jacksonville Daily Record'), 2, 'http://www.jaxdailyrecord.com/article/drone-aviations-trump-ties-untethered', '', '3/27/2017 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'Tulsa World'), 2, 'http://www.tulsaworld.com/business/technology/bits-and-bytes-the-rise-and-fall-of-tulsa-based/article_daa88f06-bdf7-5a22-b8d1-5b5e8f24fa60.html', '', '12/23/2014 12:00:00 AM', '', '', '', '', GetDate(),2)
INSERT INTO Story VALUES ((SELECT ID FROM MediaOutlet WHERE Name = 'GigaOm'), 2, 'https://gigaom.com/2011/10/11/419-patent-chaser-macrosolve-has-a-trump-card-a-deal-with-donald-trump/', '', '10/11/2011 12:00:00 AM', '', '', '', '', GetDate(),2)
