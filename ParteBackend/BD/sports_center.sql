--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2025-07-27 19:29:29

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 859 (class 1247 OID 16390)
-- Name: day_of_week_enum; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.day_of_week_enum AS ENUM (
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday',
    'Sunday'
);


ALTER TYPE public.day_of_week_enum OWNER TO postgres;

--
-- TOC entry 862 (class 1247 OID 16406)
-- Name: facility_type_enum; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.facility_type_enum AS ENUM (
    'Soccer',
    'Tennis',
    'Padel',
    'Basketball',
    'Pool',
    'Gym'
);


ALTER TYPE public.facility_type_enum OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 16450)
-- Name: facilities; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.facilities (
    facility_id integer NOT NULL,
    name character varying(15) NOT NULL,
    type character varying(20) NOT NULL,
    max_reservation_hours integer NOT NULL,
    min_reservation_hours integer DEFAULT 1 NOT NULL,
    cancellation_hours integer DEFAULT 1 NOT NULL,
    CONSTRAINT facilities_cancellation_hours_check CHECK ((cancellation_hours >= 1)),
    CONSTRAINT facilities_max_reservation_hours_check CHECK ((max_reservation_hours >= 1)),
    CONSTRAINT facilities_min_reservation_hours_check CHECK ((min_reservation_hours >= 1))
);


ALTER TABLE public.facilities OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16449)
-- Name: facilities_facility_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.facilities_facility_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.facilities_facility_id_seq OWNER TO postgres;

--
-- TOC entry 4894 (class 0 OID 0)
-- Dependencies: 223
-- Name: facilities_facility_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.facilities_facility_id_seq OWNED BY public.facilities.facility_id;


--
-- TOC entry 230 (class 1259 OID 16510)
-- Name: facility_schedules; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.facility_schedules (
    schedule_id integer NOT NULL,
    facility_id integer NOT NULL,
    day_of_week character varying(20) NOT NULL,
    opening_time time without time zone NOT NULL,
    closing_time time without time zone NOT NULL,
    CONSTRAINT chk_schedule CHECK ((closing_time > opening_time))
);


ALTER TABLE public.facility_schedules OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 16509)
-- Name: facility_schedules_schedule_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.facility_schedules_schedule_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.facility_schedules_schedule_id_seq OWNER TO postgres;

--
-- TOC entry 4895 (class 0 OID 0)
-- Dependencies: 229
-- Name: facility_schedules_schedule_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.facility_schedules_schedule_id_seq OWNED BY public.facility_schedules.schedule_id;


--
-- TOC entry 225 (class 1259 OID 16461)
-- Name: members; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.members (
    user_id integer NOT NULL,
    registration_date date NOT NULL,
    deactivation_date date,
    penalized boolean DEFAULT false,
    penalty_end_date date,
    bank_details character varying(34),
    observations text
);


ALTER TABLE public.members OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16429)
-- Name: permissions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.permissions (
    permission_id integer NOT NULL,
    description character varying(100) NOT NULL,
    code character varying(50)
);


ALTER TABLE public.permissions OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16428)
-- Name: permissions_permission_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.permissions_permission_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.permissions_permission_id_seq OWNER TO postgres;

--
-- TOC entry 4896 (class 0 OID 0)
-- Dependencies: 219
-- Name: permissions_permission_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.permissions_permission_id_seq OWNED BY public.permissions.permission_id;


--
-- TOC entry 227 (class 1259 OID 16475)
-- Name: reservations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.reservations (
    reservation_id integer NOT NULL,
    user_id integer NOT NULL,
    facility_id integer NOT NULL,
    reservation_date date NOT NULL,
    start_time time without time zone NOT NULL,
    end_time time without time zone NOT NULL,
    payment_completed boolean DEFAULT false,
    no_show boolean DEFAULT false,
    CONSTRAINT chk_reservation_time CHECK ((end_time > start_time))
);


ALTER TABLE public.reservations OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 16474)
-- Name: reservations_reservation_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.reservations_reservation_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.reservations_reservation_id_seq OWNER TO postgres;

--
-- TOC entry 4897 (class 0 OID 0)
-- Dependencies: 226
-- Name: reservations_reservation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.reservations_reservation_id_seq OWNED BY public.reservations.reservation_id;


--
-- TOC entry 228 (class 1259 OID 16494)
-- Name: role_permissions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.role_permissions (
    role_id integer NOT NULL,
    permission_id integer NOT NULL
);


ALTER TABLE public.role_permissions OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16420)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    role_id integer NOT NULL,
    role_name character varying(30) NOT NULL
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16419)
-- Name: roles_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roles_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roles_role_id_seq OWNER TO postgres;

--
-- TOC entry 4898 (class 0 OID 0)
-- Dependencies: 217
-- Name: roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roles_role_id_seq OWNED BY public.roles.role_id;


--
-- TOC entry 222 (class 1259 OID 16436)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    first_name character varying(30) NOT NULL,
    last_name character varying(60) NOT NULL,
    email character varying(100) NOT NULL,
    password character varying(100) NOT NULL,
    phone character varying(15),
    role_id integer NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16435)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 4899 (class 0 OID 0)
-- Dependencies: 221
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4683 (class 2604 OID 16453)
-- Name: facilities facility_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.facilities ALTER COLUMN facility_id SET DEFAULT nextval('public.facilities_facility_id_seq'::regclass);


--
-- TOC entry 4690 (class 2604 OID 16513)
-- Name: facility_schedules schedule_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.facility_schedules ALTER COLUMN schedule_id SET DEFAULT nextval('public.facility_schedules_schedule_id_seq'::regclass);


--
-- TOC entry 4681 (class 2604 OID 16432)
-- Name: permissions permission_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.permissions ALTER COLUMN permission_id SET DEFAULT nextval('public.permissions_permission_id_seq'::regclass);


--
-- TOC entry 4687 (class 2604 OID 16478)
-- Name: reservations reservation_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations ALTER COLUMN reservation_id SET DEFAULT nextval('public.reservations_reservation_id_seq'::regclass);


--
-- TOC entry 4680 (class 2604 OID 16423)
-- Name: roles role_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles ALTER COLUMN role_id SET DEFAULT nextval('public.roles_role_id_seq'::regclass);


--
-- TOC entry 4682 (class 2604 OID 16439)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 4882 (class 0 OID 16450)
-- Dependencies: 224
-- Data for Name: facilities; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.facilities (facility_id, name, type, max_reservation_hours, min_reservation_hours, cancellation_hours) FROM stdin;
1	Soccer 1	Soccer	3	1	1
2	Soccer 2	Soccer	2	1	1
3	Padel 1	Padel	2	1	2
4	Padel 2	Padel	2	1	1
5	Gym 1	Gym	4	1	1
6	Gym 2	Gym	3	1	1
7	Pool 1	Pool	2	1	2
8	Pool 2	Pool	1	1	1
9	Basketball 1	Basketball	3	1	1
10	Tennis 1	Tennis	2	1	1
11	Tennis 2	Tennis	2	1	1
14	Tennis 3	Tennis	3	2	1
15	Tennis 3	Tennis	3	2	1
\.


--
-- TOC entry 4888 (class 0 OID 16510)
-- Dependencies: 230
-- Data for Name: facility_schedules; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.facility_schedules (schedule_id, facility_id, day_of_week, opening_time, closing_time) FROM stdin;
1	1	Monday	09:00:00	22:00:00
2	1	Tuesday	08:00:00	20:00:00
3	1	Wednesday	08:00:00	22:00:00
4	1	Thursday	08:00:00	22:00:00
5	1	Friday	08:00:00	22:00:00
6	1	Saturday	09:00:00	20:00:00
7	1	Sunday	09:00:00	20:00:00
8	2	Monday	05:00:00	22:00:00
9	2	Tuesday	08:00:00	22:00:00
10	2	Wednesday	08:00:00	22:00:00
11	2	Thursday	08:00:00	22:00:00
12	2	Friday	08:00:00	22:00:00
13	2	Saturday	09:00:00	20:00:00
14	2	Sunday	09:00:00	20:00:00
15	3	Monday	09:00:00	21:00:00
16	3	Tuesday	09:00:00	21:00:00
17	3	Wednesday	09:00:00	21:00:00
18	3	Thursday	09:00:00	21:00:00
19	3	Friday	09:00:00	21:00:00
20	3	Saturday	10:00:00	18:00:00
21	3	Sunday	10:00:00	18:00:00
22	4	Monday	09:00:00	21:00:00
23	4	Tuesday	09:00:00	21:00:00
24	4	Wednesday	09:00:00	21:00:00
25	4	Thursday	09:00:00	21:00:00
26	4	Friday	09:00:00	21:00:00
27	4	Saturday	10:00:00	18:00:00
28	4	Sunday	10:00:00	18:00:00
29	5	Monday	07:00:00	23:00:00
30	5	Tuesday	07:00:00	23:00:00
31	5	Wednesday	07:00:00	23:00:00
32	5	Thursday	07:00:00	23:00:00
33	5	Friday	07:00:00	23:00:00
34	5	Saturday	08:00:00	20:00:00
35	5	Sunday	08:00:00	20:00:00
36	6	Monday	07:00:00	23:00:00
37	6	Tuesday	07:00:00	23:00:00
38	6	Wednesday	07:00:00	23:00:00
39	6	Thursday	07:00:00	23:00:00
40	6	Friday	07:00:00	23:00:00
41	6	Saturday	08:00:00	20:00:00
42	6	Sunday	08:00:00	20:00:00
43	7	Monday	10:00:00	20:00:00
44	7	Tuesday	10:00:00	20:00:00
45	7	Wednesday	10:00:00	20:00:00
46	7	Thursday	10:00:00	20:00:00
47	7	Friday	10:00:00	20:00:00
48	7	Saturday	11:00:00	18:00:00
49	7	Sunday	10:00:00	18:00:00
50	8	Monday	10:00:00	20:00:00
51	8	Tuesday	10:00:00	20:00:00
52	8	Wednesday	10:00:00	20:00:00
53	8	Thursday	10:00:00	20:00:00
54	8	Friday	10:00:00	20:00:00
55	8	Saturday	11:00:00	18:00:00
56	8	Sunday	11:00:00	18:00:00
57	9	Monday	08:00:00	22:00:00
58	9	Tuesday	08:00:00	22:00:00
59	9	Wednesday	08:00:00	22:00:00
60	9	Thursday	08:00:00	22:00:00
61	9	Friday	08:00:00	22:00:00
62	9	Saturday	09:00:00	20:00:00
63	9	Sunday	09:00:00	20:00:00
64	10	Monday	07:00:00	23:00:00
65	10	Tuesday	07:00:00	23:00:00
66	10	Wednesday	07:00:00	23:00:00
67	10	Thursday	07:00:00	23:00:00
68	10	Friday	07:00:00	23:00:00
69	10	Saturday	08:00:00	20:00:00
70	10	Sunday	08:00:00	20:00:00
72	14	Monday	08:00:00	22:00:00
73	15	Monday	08:00:00	22:00:00
\.


--
-- TOC entry 4883 (class 0 OID 16461)
-- Dependencies: 225
-- Data for Name: members; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.members (user_id, registration_date, deactivation_date, penalized, penalty_end_date, bank_details, observations) FROM stdin;
4	2024-01-10	\N	f	\N	ES1234567890123456789012	Observation 1
5	2024-02-15	\N	f	\N	ES9876543210987654321098	Observation 2
6	2024-03-01	\N	t	2024-03-15	ES1112223334445556667777	Observation 3
7	2024-03-05	\N	f	\N	ES5554443332221117778888	Observation 4
8	2024-03-10	\N	t	2024-03-20	ES8887776665554443332222	Observation 5
\.


--
-- TOC entry 4878 (class 0 OID 16429)
-- Dependencies: 220
-- Data for Name: permissions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.permissions (permission_id, description, code) FROM stdin;
1	Make reservations	MAKE_RESERVATIONS
2	Cancel own reservations	CANCEL_OWN_RESERVATIONS
3	Cancel others reservations limited	CANCEL_OTHERS_LIMITED
4	Cancel others reservations unlimited	CANCEL_OTHERS_UNLIMITED
5	View own reservations	VIEW_OWN_RESERVATIONS
6	View others reservations	VIEW_OTHERS_RESERVATIONS
7	Change own password	CHANGE_OWN_PASSWORD
8	Change others password	CHANGE_OTHERS_PASSWORD
9	Register as member oneself	REGISTER_SELF
10	Register anyone as member	REGISTER_ANYONE
11	Modify own member data	MODIFY_OWN_DATA
12	Modify others member data	MODIFY_OTHERS_DATA
13	Deactivate member	DEACTIVATE_MEMBER
14	Generate reports	GENERATE_REPORTS
15	View charts	VIEW_CHARTS
16	Modify roles and permissions	MODIFY_ROLES_PERMISSIONS
17	Import/export operations	IMPORT_EXPORT
18	Modify facility schedules	MODIFY_SCHEDULES
19	Have more than 1 active reservation	MULTI_ACTIVE_RESERVATIONS
20	View administration menu	VIEW_ADMIN_MENU
21	Unlimited reserve	RESERVE_UNLIMITED
22	string	string
\.


--
-- TOC entry 4885 (class 0 OID 16475)
-- Dependencies: 227
-- Data for Name: reservations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.reservations (reservation_id, user_id, facility_id, reservation_date, start_time, end_time, payment_completed, no_show) FROM stdin;
1	1	3	2025-05-28	13:00:00	14:00:00	t	f
2	2	1	0001-01-01	18:00:00	19:30:00	f	f
3	3	3	2024-03-27	09:00:00	10:30:00	t	f
4	4	1	2024-03-25	12:00:00	13:30:00	t	f
5	5	2	2024-03-26	16:00:00	17:30:00	t	f
6	6	3	2024-03-27	14:00:00	15:30:00	f	f
7	7	4	2024-03-28	10:00:00	12:00:00	t	f
8	8	5	2024-03-29	09:00:00	11:00:00	t	f
9	1	6	2024-03-30	11:00:00	12:30:00	f	f
10	2	7	2024-03-31	14:00:00	15:30:00	t	f
11	3	8	2024-03-31	16:00:00	17:30:00	f	f
12	4	9	2024-04-01	18:00:00	19:30:00	t	f
13	5	10	2024-04-02	08:00:00	09:30:00	t	f
14	6	1	2024-04-03	15:00:00	16:30:00	f	f
15	7	2	2024-04-04	12:00:00	13:30:00	t	f
16	8	5	2024-04-05	07:00:00	09:00:00	t	f
21	1	8	2025-05-16	19:00:00	20:00:00	f	f
23	1	5	2025-06-07	08:00:00	10:00:00	f	f
24	1	7	2025-06-12	15:00:00	16:00:00	f	f
25	1	7	2025-06-17	12:00:00	13:00:00	f	f
26	1	1	2025-06-04	09:00:00	10:00:00	f	f
28	1	4	2025-06-10	09:00:00	11:00:00	f	f
38	4	6	2022-05-03	22:36:00	22:52:00	t	f
39	7	4	2025-07-15	10:00:00	12:00:00	t	f
40	7	5	2025-07-15	10:00:00	12:00:00	t	f
\.


--
-- TOC entry 4886 (class 0 OID 16494)
-- Dependencies: 228
-- Data for Name: role_permissions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.role_permissions (role_id, permission_id) FROM stdin;
1	1
2	1
3	1
1	2
2	2
3	2
2	3
3	3
3	4
1	5
2	5
3	5
2	6
3	6
1	7
2	7
3	7
3	8
1	9
2	9
3	9
2	10
3	10
1	11
2	11
3	11
2	12
3	12
3	13
2	14
3	14
2	15
3	15
3	16
3	17
2	18
3	18
3	19
2	20
3	20
3	21
4	2
4	3
\.


--
-- TOC entry 4876 (class 0 OID 16420)
-- Dependencies: 218
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roles (role_id, role_name) FROM stdin;
1	Member
2	Facility Manager
3	Administrator
4	caca
\.


--
-- TOC entry 4880 (class 0 OID 16436)
-- Dependencies: 222
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, first_name, last_name, email, password, phone, role_id) FROM stdin;
1	Luis	García Márquez	luis.garcia@email.com	$2a$11$AhGwT7Qf2Z/UAGuCPSCmSOyzsFv/fh.SNZS/el5TteubI0Uj2E2Hu	600789456	3
2	Pedro	Martínez Gómez	pedro.martinez@email.com	$2a$11$4RWK9AB2hQ3FSGBMptMakOR2kL3uB9Spm.5XieQa1vJUto4hVJa3y	610234567	2
3	Laura	Hernández Pérez	laura.hernandez@email.com	$2a$11$Vuhl4PbHz.kSQsGPesBcQejyJIAL7MMb8L7G4z.KeB1SGCXiSYu5K	620345678	2
4	Juan	Pérez Sánchez	juan.perez@email.com	$2a$11$IlcwWdw1Xmh474j.a2IxnesVgv.AUgjXz2zzHONa0fdsgQUjPGoIS	600123456	1
5	Ana	Gómez Ruiz	ana.gomez@email.com	$2a$11$fEcJ.mnaUWBAqUrKevi6vuRGsU2vFJDbPIcY5cgHyCQV8FRkNjwEO	611234567	1
6	Carlos	Lopez Díaz	carlos.lopez@email.com	$2a$11$sDvQOQmyMk648z9vVcI5i.NUM5Xnonx78HqANGh.aXnISHgE0o5v6	622345678	1
7	Sofia	Ramírez Martínez	sofia.ramirez@email.com	$2a$11$VX5jiPghOzF.rnhWNeqm8eOL7MGytFHUA5kyebQSDFT04J5/b.Yy.	633456789	1
8	Pablito2	PablitoPro	proplayer@pro.com	$2a$11$Jg.uhm/TRYZHE6TnO3eBh.o5E.Mu48eiAEhsGxkxuM7Nd8OAAa/lu	234234	1
9	Laura	Martínez	laura@example.com	$2a$11$78oW.A/i5wtHstCtJsEFdO53OIgidiWjxj2LSe3ljpf/wMwGM8vbm	+34678901234	2
\.


--
-- TOC entry 4900 (class 0 OID 0)
-- Dependencies: 223
-- Name: facilities_facility_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.facilities_facility_id_seq', 15, true);


--
-- TOC entry 4901 (class 0 OID 0)
-- Dependencies: 229
-- Name: facility_schedules_schedule_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.facility_schedules_schedule_id_seq', 73, true);


--
-- TOC entry 4902 (class 0 OID 0)
-- Dependencies: 219
-- Name: permissions_permission_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.permissions_permission_id_seq', 22, true);


--
-- TOC entry 4903 (class 0 OID 0)
-- Dependencies: 226
-- Name: reservations_reservation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.reservations_reservation_id_seq', 40, true);


--
-- TOC entry 4904 (class 0 OID 0)
-- Dependencies: 217
-- Name: roles_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_role_id_seq', 3, true);


--
-- TOC entry 4905 (class 0 OID 0)
-- Dependencies: 221
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_user_id_seq', 9, true);


--
-- TOC entry 4709 (class 2606 OID 16460)
-- Name: facilities facilities_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.facilities
    ADD CONSTRAINT facilities_pkey PRIMARY KEY (facility_id);


--
-- TOC entry 4721 (class 2606 OID 16516)
-- Name: facility_schedules facility_schedules_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.facility_schedules
    ADD CONSTRAINT facility_schedules_pkey PRIMARY KEY (schedule_id);


--
-- TOC entry 4712 (class 2606 OID 16468)
-- Name: members members_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.members
    ADD CONSTRAINT members_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4701 (class 2606 OID 16434)
-- Name: permissions permissions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.permissions
    ADD CONSTRAINT permissions_pkey PRIMARY KEY (permission_id);


--
-- TOC entry 4717 (class 2606 OID 16483)
-- Name: reservations reservations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations
    ADD CONSTRAINT reservations_pkey PRIMARY KEY (reservation_id);


--
-- TOC entry 4719 (class 2606 OID 16498)
-- Name: role_permissions role_permissions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role_permissions
    ADD CONSTRAINT role_permissions_pkey PRIMARY KEY (role_id, permission_id);


--
-- TOC entry 4697 (class 2606 OID 16425)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 4699 (class 2606 OID 16427)
-- Name: roles roles_role_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_role_name_key UNIQUE (role_name);


--
-- TOC entry 4705 (class 2606 OID 16443)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 4707 (class 2606 OID 16441)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4722 (class 1259 OID 16527)
-- Name: idx_facility_schedules_facility; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_facility_schedules_facility ON public.facility_schedules USING btree (facility_id);


--
-- TOC entry 4710 (class 1259 OID 16528)
-- Name: idx_members_user; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_members_user ON public.members USING btree (user_id);


--
-- TOC entry 4713 (class 1259 OID 16526)
-- Name: idx_reservations_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_date ON public.reservations USING btree (reservation_date);


--
-- TOC entry 4714 (class 1259 OID 16525)
-- Name: idx_reservations_facility; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_facility ON public.reservations USING btree (facility_id);


--
-- TOC entry 4715 (class 1259 OID 16524)
-- Name: idx_reservations_user; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_user ON public.reservations USING btree (user_id);


--
-- TOC entry 4702 (class 1259 OID 16522)
-- Name: idx_users_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_users_email ON public.users USING btree (email);


--
-- TOC entry 4703 (class 1259 OID 16523)
-- Name: idx_users_role; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_users_role ON public.users USING btree (role_id);


--
-- TOC entry 4729 (class 2606 OID 16517)
-- Name: facility_schedules fk_facility_schedules_facility; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.facility_schedules
    ADD CONSTRAINT fk_facility_schedules_facility FOREIGN KEY (facility_id) REFERENCES public.facilities(facility_id) ON DELETE CASCADE;


--
-- TOC entry 4724 (class 2606 OID 16469)
-- Name: members fk_members_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.members
    ADD CONSTRAINT fk_members_user FOREIGN KEY (user_id) REFERENCES public.users(user_id) ON DELETE CASCADE;


--
-- TOC entry 4725 (class 2606 OID 16489)
-- Name: reservations fk_reservations_facility; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations
    ADD CONSTRAINT fk_reservations_facility FOREIGN KEY (facility_id) REFERENCES public.facilities(facility_id) ON DELETE CASCADE;


--
-- TOC entry 4726 (class 2606 OID 16484)
-- Name: reservations fk_reservations_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.reservations
    ADD CONSTRAINT fk_reservations_user FOREIGN KEY (user_id) REFERENCES public.users(user_id) ON DELETE CASCADE;


--
-- TOC entry 4727 (class 2606 OID 16504)
-- Name: role_permissions fk_role_permissions_permission; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role_permissions
    ADD CONSTRAINT fk_role_permissions_permission FOREIGN KEY (permission_id) REFERENCES public.permissions(permission_id) ON DELETE CASCADE;


--
-- TOC entry 4728 (class 2606 OID 16499)
-- Name: role_permissions fk_role_permissions_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role_permissions
    ADD CONSTRAINT fk_role_permissions_role FOREIGN KEY (role_id) REFERENCES public.roles(role_id) ON DELETE CASCADE;


--
-- TOC entry 4723 (class 2606 OID 16444)
-- Name: users fk_users_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT fk_users_role FOREIGN KEY (role_id) REFERENCES public.roles(role_id) ON DELETE RESTRICT;


-- Completed on 2025-07-27 19:29:29

--
-- PostgreSQL database dump complete
--

