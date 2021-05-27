ALTER TABLE Emailing DROP COLUMN HtmlDetail
ALTER TABLE Emailing DROP COLUMN TextDetail

ALTER TABLE Emailing ADD HtmlDetail image NULL
ALTER TABLE Emailing ADD TextDetail image NULL

ALTER TABLE EmailQue DROP COLUMN HtmlDetail
ALTER TABLE EmailQue DROP COLUMN TextDetail

ALTER TABLE EmailQue ADD HtmlDetail image NULL
ALTER TABLE EmailQue ADD TextDetail image NULL

ALTER TABLE Impressum DROP COLUMN Detail

ALTER TABLE Impressum ADD Detail image NULL