from newspaper import Article
import pyodbc

#print(pyodbc.drivers())

#conn = pyodbc.connect('Driver={SQL Server};SCOTT-PC\\SQLExpress;;Database=myDB;Trusted_Connection=yes;')

conn = pyodbc.connect(
    r'DRIVER={SQL Server Native Client 11.0};'
    r'SERVER=SCOTT-PC\SQLExpress;'
    r'DATABASE=Trump;'
    r'Trusted_Connection=yes;'
    )

cursor = conn.cursor()

def import_story(id, url):
	try:
		article = Article(url)
		article.download()
		article.parse()
		article.nlp()
	except:
		return None
	
	print('')
	print(id)
	#print(article.keywords)

	cursor.execute("UPDATE Story SET TopImage = ? WHERE ID = ?", article.top_image, id)
	conn.commit()
	#input("After TopImage")

	cursor.execute("UPDATE Story SET Authors = ? WHERE ID = ?", ', '.join(article.authors), id)
	conn.commit()
	#input("After Authors")
		
	#cursor.execute("UPDATE Story SET Keywords = ? WHERE ID = ?", article.keywords, id)
	#conn.commit()
	#input("After Keywords")

	cursor.execute("UPDATE Story SET Body = ? WHERE ID = ?", article.text, id)
	conn.commit()
	#input("After Body")
		
	cursor.execute("UPDATE Story SET Title = ? WHERE ID = ?", article.title, id)
	conn.commit()
	#input("After Title")



	print(article.authors)
	#cursor.execute("UPDATE Story SET Authors = ? WHERE ID = ?", article.authors, id)
	#conn.commit()


cursor.execute("SELECT ID, Link FROM Story")
rows = cursor.fetchall()
for row in rows:
    import_story(row.ID, row.Link)

input("After Query")


    
#url = 'https://www.washingtonpost.com/politics/trump-mortgage-failed-heres-what-that-says-about-the-gop-front-runner/2016/02/28/f8701880-d00f-11e5-88cd-753e80cd29ad_story.html?utm_term=.35807dc39638'
#article = Article(url)
#article.download()
#article.parse()

#print(article.authors)
#print(article.publish_date)
#print(article.top_image)


#article.nlp()
#print(article.keywords)
#print(article.summary)

#print(article.text)
