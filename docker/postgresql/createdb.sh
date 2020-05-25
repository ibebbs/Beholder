#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE TABLE faces (id uuid PRIMARY KEY, uri varchar(255), created timestamp);
    INSERT INTO faces (id, uri, created) VALUES('b32b7afc-c394-4f08-a07f-b9fd48f095cf', 'http://192.168.1.22:10000/devstoreaccount1/unrecognised/0094fcbf-6d5e-4b05-a39b-f5c209b46379.png', '2020-05-24T09:35:40Z');

    CREATE TABLE recogniser (id uuid PRIMARY KEY, name varchar(80));
    INSERT INTO recogniser (id, name) VALUES ('040c6e46-3bb3-488b-b4de-4b9a5e94c5de', 'ml.net');
    INSERT INTO recogniser (id, name) VALUES ('3d6da754-a9ec-457c-9002-125efab35781', 'ian');
    INSERT INTO recogniser (id, name) VALUES ('60893fd9-02ee-449a-a2e1-309cce9803b5', 'rachel');
    INSERT INTO recogniser (id, name) VALUES ('1aae3457-f86c-45c6-ae7f-935a68c77df5', 'mia');
    INSERT INTO recogniser (id, name) VALUES ('f1704e03-fb90-473c-ae49-44c19a71e14a', 'max');

    CREATE TABLE recognition (
        id uuid PRIMARY KEY, 
        face_id uuid references faces(id), 
        recogniser_id uuid references recogniser(id), 
        label varchar(80), 
        confidence float, 
        created timestamp
    );
EOSQL