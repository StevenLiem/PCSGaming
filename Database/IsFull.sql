-- Drop Table
drop table GAME cascade constraint purge;
drop table DEVELOPER cascade constraint purge;
drop table PUBLISHER cascade constraint purge;
drop table GENRE cascade constraint purge;
drop table TRANSACTION cascade constraint purge;
drop table MEMBER cascade constraint purge;
drop table BUNDLE cascade constraint purge;
drop table BUNDLE_GAME cascade constraint purge;
drop table TOKEN_CONTENTS cascade constraint purge;
drop table GAME_TRANSACTION cascade constraint purge;
drop table GAME_BUNDLE cascade constraint purge;

-- Create Table
create table GAME(
	GAME_ID varchar2(5) primary key,
	DEVELOPER_ID varchar2(5),
	PUBLISHER_ID varchar2(5),
	GENRE_ID varchar2(5),
	NAME varchar2(100),
    RELEASE_DATE date,
	PRICE number,
	STOCK number,
	IS_ACTIVE_GAME number(1)
);
--active games has IS_ACTIVE_GAME valued 1, inactive games valued 0
create table DEVELOPER(
	DEVELOPER_ID varchar2(5) primary key,
	NAME varchar2(50)
);
create table PUBLISHER(
	PUBLISHER_ID varchar2(5) primary key,
	NAME varchar2(50)
);
create table GENRE(
	GENRE_ID varchar2(5) primary key,
	NAME varchar2(50)
);
create table TRANSACTION(
	TOKEN varchar2(10) primary key,
	MEMBER_ID varchar2(5),
	TRANSACTION_DATE date,
	TOTAL number
);
create table MEMBER(
	MEMBER_ID varchar2(5) primary key,
	REAL_NAME varchar2(50),
	USERNAME varchar2(25),
	PASSWORD varchar2(50),
	BIRTH_DATE date,
	JOINED_DATE date
);
create table BUNDLE(
	BUNDLE_ID varchar2(5) primary key,
	NAME varchar2(50),
	PRICE number,
    DISCOUNT number,
	IS_ACTIVE number(1)
);
create table BUNDLE_GAME(
	BUNDLE_ID varchar2(5),
	GAME_ID varchar2(5)
);
-- CONTENT_ID bisa game id atau bundle id
create table TOKEN_CONTENTS(
	TOKEN_ID varchar2(10),
	CONTENT_ID varchar2(5),
	MEMBER_ID varchar(5),
	QTY number
);
create table GAME_TRANSACTION(
	GAME_ID varchar2(5),
	TOKEN varchar2(10),
	PRICE number,
	QTY number,
	SUBTOTAL number
);
-- create table GAME_GENRE(
-- 	GAME_ID varchar2(5),
-- 	GENRE_ID varchar2(5)
-- );

-- Function --
create or replace FUNCTION GENERATE_GAME_ID
(
    game_name in varchar2
)
return varchar2
is
    gameID varchar2(5);
    spaceIndex number;
    ctr number;
begin
    spaceIndex:=instr(game_name,' ');
    if spaceIndex > 0 and length(game_name)>spaceIndex then
        gameID:=substr(game_name,1,1)||substr(game_name,instr(game_name,' ')+1,1);
    else
        gameID:=substr(game_name,1,2);
    end if;
    gameID:=upper(gameID);
    select count(GAME_ID) into ctr from GAME where GAME_ID like gameID||'%';
    gameID:=gameID||lpad(ctr+1,3,'0');

    return gameID;
end;
/

-- Trigger --
create or replace trigger TR_MEMBER_BI
before insert on MEMBER
for each row
declare
    mem_id MEMBER.MEMBER_ID%type;
    spaceIndex number;
    ctr number;
begin
    spaceIndex:=instr(:NEW.REAL_NAME,' ');
    if spaceIndex > 0 then
        mem_id:=substr(:NEW.REAL_NAME,1,1)||substr(:NEW.REAL_NAME,instr(:NEW.REAL_NAME,' ')+1,1);
    else
        mem_id:=substr(:NEW.REAL_NAME,1,2);
    end if;
    select count(MEMBER_ID) into ctr from MEMBER where MEMBER_ID like mem_id||'%';
    :NEW.MEMBER_ID:=upper(mem_id)||lpad(ctr+1,3,'0');

    if :NEW.JOINED_DATE is null then
        select CURRENT_DATE into :NEW.JOINED_DATE from dual;
    end if;
end;
/

create or replace trigger TR_GAME_TRANSACTION
before insert on GAME_TRANSACTION
for each row
declare
    ctr number;
begin
    select STOCK into ctr from GAME where GAME_ID = :NEW.GAME_ID;
    ctr := ctr - :NEW.QTY;
    UPDATE GAME SET STOCK = ctr WHERE GAME_ID = :NEW.GAME_ID;
end;
/

-- Insert Data
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE001','Moon Studios GmbH');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE002','Team Cherry');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE003','Ubisoft');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE004','Valve');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE005','Square Enix');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE006','SEGA');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE007','Rockstar Games');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE008','CAPCOM');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE009','CD PROJEKT RED');
insert into DEVELOPER(DEVELOPER_ID, NAME) values('DE010','id Software');

insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU001','Xbox Game Studios');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU002','Team Cherry');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU003','Ubisoft');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU004','Valve');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU005','Square Enix');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU006','SEGA');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU007','Rockstar Games');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU008','CAPCOM');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU009','CD PROJEKT RED');
insert into PUBLISHER(PUBLISHER_ID, NAME) values('PU010','Bethesda Softworks');

insert into GENRE(GENRE_ID, NAME) values('GE001','Action');
insert into GENRE(GENRE_ID, NAME) values('GE002','Adventure');
insert into GENRE(GENRE_ID, NAME) values('GE003','Casual');
insert into GENRE(GENRE_ID, NAME) values('GE004','Fighting');
insert into GENRE(GENRE_ID, NAME) values('GE005','Platform');
insert into GENRE(GENRE_ID, NAME) values('GE006','Puzzle');
insert into GENRE(GENRE_ID, NAME) values('GE007','Racing');
insert into GENRE(GENRE_ID, NAME) values('GE008','Role-Playing');
insert into GENRE(GENRE_ID, NAME) values('GE009','Shooter');
insert into GENRE(GENRE_ID, NAME) values('GE010','Simulation');
insert into GENRE(GENRE_ID, NAME) values('GE011','Sports');
insert into GENRE(GENRE_ID, NAME) values('GE012','Strategy');
insert into GENRE(GENRE_ID, NAME) values('GE013','Fantasy');

insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('OA001', 'DE001', 'PU001', 'GE001', 'Ori and the Blind Forest: Definitive Edition', TO_DATE('27/04/2016','DD/MM/YYYY'), 40000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('OA002', 'DE001', 'PU001', 'GE001', 'Ori and the Will of the Wisps', TO_DATE('11/04/2020','DD/MM/YYYY'), 140000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('HK001', 'DE002', 'PU002', 'GE002', 'Hollow Knight', TO_DATE('25/02/2017','DD/MM/YYYY'), 116000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('TC001', 'DE003', 'PU003', 'GE009', 'Tom Clancys Rainbow Six Siege', TO_DATE('02/12/2015','DD/MM/YYYY'), 205000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('WD001', 'DE003', 'PU003', 'GE001', 'Watch Dogs', TO_DATE('26/05/2014','DD/MM/YYYY'), 309000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('WD002', 'DE003', 'PU003', 'GE001', 'Watch Dogs 2', TO_DATE('28/11/2016','DD/MM/YYYY'), 619000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('FC001', 'DE003', 'PU003', 'GE001', 'Far Cry 4', TO_DATE('18/11/2014','DD/MM/YYYY'), 309000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('FC002', 'DE003', 'PU003', 'GE001', 'Far Cry 5', TO_DATE('27/03/2018','DD/MM/YYYY'), 619000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('FC003', 'DE003', 'PU003', 'GE001', 'Far Cry New Dawn', TO_DATE('16/02/2019','DD/MM/YYYY'), 465000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('L4001', 'DE004', 'PU004', 'GE009', 'Left 4 Dead', TO_DATE('17/11/2008','DD/MM/YYYY'), 70000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('L4002', 'DE004', 'PU004', 'GE009', 'Left 4 Dead 2', TO_DATE('17/11/2009','DD/MM/YYYY'), 70000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('PO001', 'DE004', 'PU004', 'GE006', 'Portal', TO_DATE('10/10/2007','DD/MM/YYYY'), 70000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('P2001', 'DE004', 'PU004', 'GE006', 'Portal 2', TO_DATE('19/04/2011','DD/MM/YYYY'), 70000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('FF001', 'DE005', 'PU005', 'GE002', 'Final Fantasy IX', TO_DATE('14/04/2016','DD/MM/YYYY'), 200000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('FF002', 'DE005', 'PU005', 'GE002', 'Final Fantasy XV', TO_DATE('07/03/2018','DD/MM/YYYY'), 695000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('YK001', 'DE006', 'PU006', 'GE004', 'Yakuza Kiwami', TO_DATE('19/02/2019','DD/MM/YYYY'), 260000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('YK002', 'DE006', 'PU006', 'GE004', 'Yakuza Kiwami 2', TO_DATE('09/05/2019','DD/MM/YYYY'), 390000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('GT001', 'DE007', 'PU007', 'GE001', 'Grand Theft Auto V', TO_DATE('14/04/2015','DD/MM/YYYY'), 290000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('RD001', 'DE007', 'PU007', 'GE002', 'Red Dead Redemption 2', TO_DATE('06/12/2019','DD/MM/YYYY'), 640000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('RE001', 'DE008', 'PU008', 'GE002', 'Resident Evil 2', TO_DATE('25/01/2019','DD/MM/YYYY'), 320000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('RE002', 'DE008', 'PU008', 'GE002', 'Resident Evil 3', TO_DATE('03/04/2020','DD/MM/YYYY'), 825000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('TW001', 'DE009', 'PU009', 'GE002', 'The Witcher 3', TO_DATE('18/05/2015','DD/MM/YYYY'), 360000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('C2001', 'DE009', 'PU009', 'GE001', 'Cyberpunk 2077', TO_DATE('10/12/2020','DD/MM/YYYY'), 700000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('DO001', 'DE010', 'PU010', 'GE009', 'DOOM', TO_DATE('12/05/2016','DD/MM/YYYY'), 266000, 50, 1);
insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) values('DE001', 'DE010', 'PU010', 'GE009', 'DOOM Eternal', TO_DATE('20/03/2020','DD/MM/YYYY'), 800000, 50, 1);

-- insert into GAME_GENRE(GAME_ID, GENRE_ID) values('RD001','GE001');
-- insert into GAME_GENRE(GAME_ID, GENRE_ID) values('RD001','GE002');
-- insert into GAME_GENRE(GAME_ID, GENRE_ID) values('YK001','GE001');
-- insert into GAME_GENRE(GAME_ID, GENRE_ID) values('YK001','GE002');

insert into MEMBER(REAL_NAME, USERNAME, PASSWORD, BIRTH_DATE, JOINED_DATE) values('Mason Williams', 'mwilliams','freegamespls',TO_DATE('19/08/1999','DD/MM/YYYY'),TO_DATE('07/10/2020','DD/MM/YYYY'));
insert into MEMBER(REAL_NAME, USERNAME, PASSWORD, BIRTH_DATE) values('Mike Wallace', 'mwallace','yesyesyes',TO_DATE('06/09/1969','DD/MM/YYYY'));

insert into BUNDLE(BUNDLE_ID, NAME, PRICE, DISCOUNT, IS_ACTIVE) values('BDL01', 'Watch Dog Pack', 928000, 30, 1);

insert into BUNDLE_GAME(BUNDLE_ID, GAME_ID) values('BDL01', 'FC001');
insert into BUNDLE_GAME(BUNDLE_ID, GAME_ID) values('BDL01', 'FC002');

commit;