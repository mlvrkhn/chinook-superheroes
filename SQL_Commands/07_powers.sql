INSERT INTO Power (Name, Description)
VALUES 
    ('Magic', 'Ability to cast spells and manipulate magical forces'),
    ('Nature Affinity', 'Deep connection with nature and ability to communicate with animals'),
    ('Wind Riding', 'Ability to glide on wind currents and communicate with the wind');

UPDATE Superhero
SET PowerId = 1
WHERE Name = 'Howl Jenkins Pendragon';

UPDATE Superhero
SET PowerId = 2
WHERE Name = 'San';

UPDATE Superhero
SET PowerId = 3
WHERE Name = 'Nausicaä';
