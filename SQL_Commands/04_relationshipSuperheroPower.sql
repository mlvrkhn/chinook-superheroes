ALTER TABLE Superhero
ADD PowerId INT;

ALTER TABLE Superhero
ADD CONSTRAINT fk_Superhero_Power
FOREIGN KEY (PowerId)
REFERENCES Power(Id);
