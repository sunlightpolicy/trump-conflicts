USE Trump
GO


INSERT INTO MediaOutlet VALUES ('Press Release')
INSERT INTO MediaOutlet VALUES ('Taiwan News')
INSERT INTO MediaOutlet VALUES ('Reuters')
INSERT INTO MediaOutlet VALUES ('USA Today')
INSERT INTO MediaOutlet VALUES ('The Hill')
INSERT INTO MediaOutlet VALUES ('Chicago Tribune')
INSERT INTO MediaOutlet VALUES ('Office of Government Ethics')
INSERT INTO MediaOutlet VALUES ('The Guardian')
INSERT INTO MediaOutlet VALUES ('Palm Beach Post')
INSERT INTO MediaOutlet VALUES ('Daily Beast')
INSERT INTO MediaOutlet VALUES ('MarketWatch')
INSERT INTO MediaOutlet VALUES ('The New Republic')
INSERT INTO MediaOutlet VALUES ('WAMU')



INSERT INTO SystemUser VALUES (2, 'rggoldstraw@gmail.com', 'Roger', 'Goldstraw')
INSERT INTO SystemUser VALUES (2, 'hjohnso3@kent.edu', 'Heidi', 'Johnson')
INSERT INTO SystemUser VALUES (1, 'scottmckissock@gmail.com', 'Scott', 'McKissock')

UPDATE SystemUser SET FirstName = 'Bob' WHERE Email = 'rggoldstraw@gmail.com'



SELECT * FROM Systemuser 