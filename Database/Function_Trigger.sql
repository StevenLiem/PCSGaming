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