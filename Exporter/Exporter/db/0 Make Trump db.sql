

-- Make a directory c:\TrumpDb first, or this won't work 


-- Comment this out if db doesn't exist
ALTER DATABASE Trump SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
DROP Database Trump
GO



CREATE DATABASE Trump
 CONTAINMENT = NONE
 ON  PRIMARY
( NAME = N'Trump', FILENAME = N'C:\TrumpDb\Trump.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON
( NAME = N'Trump_Log', FILENAME = N'C:\TrumpDb\Trump_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO