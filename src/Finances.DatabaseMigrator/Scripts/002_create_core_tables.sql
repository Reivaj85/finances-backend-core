create table if not exists core.households (
    id uuid primary key,
    name varchar(160) not null,
    created_at timestamp with time zone not null default now()
);

create table if not exists core.categories (
    id uuid primary key,
    name varchar(120) not null,
    created_at timestamp with time zone not null default now()
);
