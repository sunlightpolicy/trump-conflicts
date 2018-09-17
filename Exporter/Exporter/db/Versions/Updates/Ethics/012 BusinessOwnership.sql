USE Trump
GO
INSERT INTO BusinessOwnership (OwnerID, OwneeID, OwnershipPercentage) VALUES ((SELECT ID FROM Business WHERE Name = 'Ivanka OPO Hotel Manager LLC (Management of DC Hotel)'), (SELECT ID FROM Business WHERE Name = 'OPO Hotel Manager LLC (Management of DC Hotel)'), '')
INSERT INTO BusinessOwnership (OwnerID, OwneeID, OwnershipPercentage) VALUES ((SELECT ID FROM Business WHERE Name = 'Ivanka OPO Hotel Manager LLC (Management of DC Hotel)'), (SELECT ID FROM Business WHERE Name = 'OPO Hotel Manager LLC (Management of DC Hotel)'), '0.0')
