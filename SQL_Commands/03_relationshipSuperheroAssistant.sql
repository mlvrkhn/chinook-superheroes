ALTER TABLE Assistant
ADD SuperheroId INT;

ALTER TABLE Assistant
ADD CONSTRAINT fk_Assistant_Superhero 
FOREIGN KEY (SuperheroId)
REFERENCES Superhero(Id);
