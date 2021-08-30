﻿using System;
using BoardC;
using PlayerC;
using System.Threading;
using System.IO;

namespace GameLoopC
{
    class GameLoop
    {
        public Board b;
        string path = "C:/Users/Jelle/Documents/School UM/Practice C#/Checkers - Console/Checkers/";

         public void setup()
        {
            // Ask which players to use
            int[] players = askPlayers();

            // Create a board
            this.b = new Board();

            // Assign player
            this.b.player1 = loadPlayer(players[0], 1);
            this.b.player2 = loadPlayer(players[1], 2);
            this.b.player1.B = this.b;
            this.b.player2.B = this.b;
            this.b.currentPlayer = this.b.player1;
        }

        private int[] askPlayers()
        {
            // Print intro text
            Console.WriteLine(File.ReadAllText(path + "Text/setup.txt"));

            // Receive player answers
            int[] players = new int[2];
            Console.Write("\nPlayer 1: ");
            players[0] = Convert.ToInt32(Console.ReadLine());
            Console.Write("Player 2: ");
            players[1] = Convert.ToInt32(Console.ReadLine());

            // Give instruction for the Human player
            if (players[0] == 1 || players[1] == 1)
                Console.WriteLine(File.ReadAllText(path + "Text/human.txt"));

            return players;
        }

        private Player loadPlayer(int n, int id)
        {
            if (n == 1)
                return new Human(id);
            else if (n == 2)
                return new RandomBot(id);
            else if (n == 3)
            {
                if (id == 1)
                {
                    return new NNBot(1, new int[] { 21 }, new double[] { 0.03299799516471, -0.857639713379387, 0.387878145970348, 0.0156577529924257, -1.32418078734734, -0.83127629690863, 0.347245136204756, 0.452696991713111, -0.984020506140786, -1.0726306847402, -0.25023112259816, 0.80733424625701, 0.775670472777295, 0.281396572259905, 0.010653234208307, 0.488184464391407, 0.0173993690486065, -0.715289186902945, -0.143924592013436, -0.958393100047667, 0.0302546166490086, 0.121355931331104, -0.0732332915408694, -1.00402563903668, 1.14922425798104, 1.48290392697458, 1.25678327575176, -1.1150036794669, 0.147611378062335, 1.05693584415453, -0.0136508361965653, -0.435447955823246, 0.204274283933581, 0.284173903304233, -0.278649687896785, 0.375081645732318, -0.370778132868362, -0.316486050871427, 0.143505907335088, -2.1345807491497, -0.224240711063259, -0.274109883827209, -0.916426079658058, 0.323210262145479, -0.290717944288961, -0.602932802449415, -0.834827258523939, -1.1609436340681, 0.714911750035785, -1.75398718263208, 0.221048583845165, 0.823544787510086, 0.486273169743956, 0.100030511664241, -0.0622767779567636, -0.522122869744023, 0.160620122640682, -0.10453386004294, -0.0337704731075886, -2.12128528725416, -0.344293864604223, -0.196046119181461, -1.48001240006649, 0.886852323374177, 0.433204839091378, -0.498761553200316, -0.321727841776669, -0.876166307891797, 0.372448473992966, -1.14327340428404, 0.707061878734763, 0.138672406733349, 0.986847714398451, 0.0360248040109991, 0.34727547869425, -0.364666499018048, 0.726565422595742, -0.446615986850399, 1.01270522154528, 0.33378389912368, -0.596735859218396, 0.495013706849429, -1.32380102508413, -0.579144149240639, 0.324603966937682, -2.69793492483345, -1.32117731721195, -1.33701018445986, 0.977682647052073, -1.24339626915445, 0.360095069329299, -1.83637677998579, -0.615183023556686, -1.32313042498339, -0.373488902055421, -0.920663111107733, -0.627983990650617, -0.0660714197745876, -1.1264625386691, 0.287498880777275, -0.346630640256512, -0.534283925096637, -1.04316667446083, 0.617669309311392, -0.714068696235338, -2.2065542284197, 0.471580263679652, 0.506918287489991, 0.96724708819168, -0.875809682940976, 0.64350957860868, -0.240736090224579, 1.92334182766422, 1.23836166003643, -0.015433479689729, 1.02591246484123, -0.139228615974648, 1.75822700083173, -1.95627286597447, 0.404510236766427, -0.00808032253202065, 0.768598106349166, -1.53274335876701, -0.0214519298269658, 0.456759470378402, -0.649068972863755, -0.270774343293521, 0.143232188487068, 1.85310305520105, 1.00805496017358, -0.0988734103035522, -0.687938921008231, -1.28221393901958, -0.0281678310260958, -0.153277870455421, 0.192644390134441, 1.37427205155803, 1.88794261211899, -0.590534192784938, 1.18225072286197, -1.27675262292696, -0.69249097942025, 0.0601269065216776, 1.1514552892891, -0.716397383467479, 0.18936565259442, 1.32417272838027, 0.233514053622965, -0.563617570704603, 0.00130909041096886, 0.6527661532875, -1.08413034297253, 0.884030416088193, 0.925863010425057, -1.47744074928455, -1.14333322429719, -0.645738331319643, -0.260832417179287, 1.11987728910515, -1.04883070187589, -0.0830651320671036, -1.17316623191497, 1.23234158893225, -0.741008062842771, 1.18051161997975, -0.973691901761895, -0.227894732601892, 0.891236223346198 }); ;
                }
                else
                {
                    return new NNBot(2, new int[] { 21 }, new double[] { -0.409390517817526, 0.233492213759335, 0.378895185715004, -0.664149614011007, 0.0788156973797902, -0.418406006260033, 0.0733958828837592, -0.468083645085843, -0.247403089887185, -0.582362104827707, 0.269534469824068, -1.06998603782616, 0.512823343632195, -0.193448042237874, 0.0140044818464687, -0.334861681486881, 0.577902979952238, 0.367080317631867, -0.0479183075008534, -0.607278253583833, -0.679849503063527, -0.601647821185946, 0.553702536413308, -0.574528138304375, -1.03855881480899, 0.0444984574776601, 0.335483036276644, -0.602558523115031, -1.01923429862095, 0.305342902455173, -0.258078242306634, 0.683286314799118, -0.427928292973865, -0.454898469711141, -0.306141253354094, -0.0329700115988822, 0.162823538720991, -0.587747257127309, 0.37552565109242, 1.08170552078248, 0.496185067457233, -0.388281204336454, 0.118632862981704, -0.439576174802881, 0.689948117565339, -0.862368661264129, 0.689057217184015, 0.615777402820893, 0.374452756077262, 0.0409697770564676, -0.306404251398707, 0.181532919142178, 0.074130261700661, -0.209998352900147, 1.02685268585424, -0.90028145776609, 0.609602923905292, -0.129131066323785, -0.944733103362253, 1.02703146928783, 0.00337436387938184, -0.399786117556405, -0.73024905076262, 0.101272017858583, 0.372901812951501, 0.977068903030394, 0.395960898905043, 0.350807690807063, -0.647890604519234, 0.982124976921885, 0.47732719673185, 0.424659208475919, 0.67549133786256, 0.0175030886975597, -0.314829152107625, 1.04145129341234, 0.740689054825664, 0.375974619121279, -0.270390881933454, 0.839009060333022, 0.896592233398274, 0.687951983156592, 0.389698926424467, 0.421494392525169, -0.476915827825161, 0.612276435765567, -0.530204507652765, 0.529002151581925, -0.398998030065092, 0.145720248760525, 0.496669970101989, -0.755749129925738, -0.986629736719946, 0.557113310465176, 0.113153213571363, 0.309848607312817, -0.725585942843736, 0.672533537225115, 0.252577985172429, 0.781215598704859, -0.566645578861537, -0.438459513098215, 0.899185903905512, 0.568079155785069, 0.289722170466428, 0.702793799085912, 0.5309640633785, 0.834335546048514, 1.19078612697347, -0.0840702188825562, 0.151893615444141, -0.717621638308103, 0.462604953540771, 0.246018870033333, -1.17005704432729, 0.645483859533204, 0.391125619942847, -0.455375775091991, -0.327207559057142, 0.635505920688392, 0.200226998631948, -0.730235449029242, 0.512037114944326, -1.09472039474394, -0.141356066284401, 0.0897402144455073, 0.686203374963348, 0.264393661643562, 0.0452779144026703, -0.990661338316585, -0.615126398794878, -0.502350637341082, 0.991208137125339, -0.775229244039035, -0.580834531844982, 0.359716069586443, -0.723411517903866, 0.419377746605025, -0.908295647780549, -0.0581605858440327, -0.697292858593768, -0.575948815246089, 1.03220223019933, 0.361140675894516, 0.484087568351108, 1.04325175345561, 0.620649485672195, -0.358689810432815, -1.13447657571383, 0.595684518732915, 0.70494301847878, 1.09900047576474, 0.421220717565725, 0.676124080515524, -0.135225586539705, -0.572155789855009, -0.0157580931045852, -0.416935557228017, -0.341631115456871, 0.484572686131379, 0.886092884529425, -0.415066356614729, -0.591617762735867, -0.01263265766326, -0.435329345839717, -0.290926814098343, -0.528324809753487, -0.396075736100821 });
                }
            }
            else
            {
                if (id == 1)
                {
                    return new AlphaBetaBot(1, 3, new int[] { 21 }, new double[] { -1.21236966502032, 0.347685280068631, -0.806594603139253, -0.641594820838233, -0.309555339771116, 0.557733455350498, -0.345399721220787, -0.353827974108899, -0.456239938599169, -1.05502967655427, 0.196130202592411, 1.25818399165672, 0.618593554184117, 0.273629311716011, -0.266141008593208, -0.599490957380967, 0.46023029413085, 1.52111140301969, 0.159746675128, 0.227221911296817, -0.0127764570120615, -1.50222672394115, -0.573085798683151, 0.662734848173677, 0.948773148329357, -0.306908991796388, 0.00155968871040257, -0.147274994476361, 0.372894600673064, -0.0824618577409824, -0.305375388383574, 0.35962773561926, -0.165199001513048, 1.10873587353096, 0.121689996622358, -0.628493203724964, -0.591887723115221, -0.182914300906898, 0.32630751634776, 0.211727330792568, -1.46102354208055, -0.624604794720469, -0.401573413238662, -0.79610798905888, -0.221680811709576, 0.845414636118996, -0.149084451328537, -1.63395887957139, 1.26514470682719, -1.6015880315572, -0.000180407194504711, 1.05377092785378, -0.391752017960768, -0.352043803479543, -0.195055608961292, 1.0957483188695, 0.145389487452521, 0.82632394697346, -1.27409753344213, 0.807481378343646, 1.12952243391402, 0.25949612062401, -0.200464378320828, 0.0726180840621786, -0.949620071425857, 0.20384213826798, -1.25230053824014, -1.26585276437265, -1.31907551657365, -0.951681340905689, -1.63882269251105, 1.24878374184425, -0.943853729611194, 1.02656932642058, -0.234028602942838, -0.302999222210142, -1.26214405685297, -0.0450017143948942, -0.286984232038718, -0.178629862344186, -0.808090457370547, 0.755443403499873, 0.979693877384856, -0.885582323714896, 0.620896016769528, 1.70359982070681, -0.594236152523307, -1.18760170598403, 0.450090080825654, -1.11803827277293, 1.08408641248666, 1.45080193479117, -0.616353396264069, -0.5675710670499, 0.324541808955624, 1.0340112135904, -0.130077153388447, 0.574309602181571, 0.00322821084560271, 0.484662606722984, 0.633509339361223, -0.357695566889688, -0.146247468956861, -0.00758279068748602, -0.368773895138304, -0.224439190223086, 0.815362012626306, -0.301200461481326, 1.37903237907637, -0.0825402762659546, 0.259839826850146, -1.5547225185692, 1.30827740955086, 1.20166186392385, -0.677965215257353, 0.607488934815623, -1.37620402773665, -0.826630424976643, 0.432954990623032, -0.167433741464016, 1.24767271394267, 2.44894573555745, -1.03990819644644, -0.169363831598016, 0.526528727857642, -0.305404713868818, -1.2562715963024, 0.242576639630169, 0.248697415924956, -0.523257375705641, -0.125781527895379, -0.279784273137238, 1.72768087078709, 1.29155575951634, 0.119410252021351, 1.01961493388266, 1.12055006023522, -1.30024588645913, -0.502393214382414, -0.570522168101986, -0.68267347881695, 1.01874783519597, -0.246790069386731, -1.28371398024434, 0.919125277162122, 0.848847844218299, 0.483642289989461, -1.24685718538093, -0.0603016329977203, 0.0121155773578751, -0.762210420804196, -0.51615306211922, 0.744130970441797, 0.78837229464593, 0.448926323651768, 0.726800373744592, -0.914252498263611, 0.432189569660551, 0.290035197413543, -0.529654379947881, 0.323550717590168, -0.116532409385095, 0.0431608676645722, -0.0678118928651381, -1.49507775041046, 1.00623585121065, 0.442660724251839, -1.03669544811672 });
                }
                else
                {
                    return new AlphaBetaBot(2, 3, new int[] { 21 }, new double[] { -0.0134702652057028, -0.324839065351914, -0.523217291465596, -0.113800725254137, -0.11643637128474, 0.519180010896726, 0.899026262061217, -0.064813821955963, 0.601996002300641, -0.0191890448421186, -0.713606155809763, -0.210707081579932, 1.73374574979942, -0.426629159216131, -0.559009519316726, 1.45970266799429, -1.10307267825262, -1.19760938556754, 0.363530452183229, 0.522164184494021, 0.603228773504137, -0.387764290039318, -0.249045929987471, -0.863448399753984, -0.743247690840274, 0.0154539528374812, -0.126126500254556, -1.06372559271461, -1.20147624143934, 1.39279278805144, 1.03672934278693, -0.861312742513284, -0.108601418141556, 0.468740526921461, 0.207169646493704, 0.379186955690005, -1.01511080307658, -1.30398144796676, -0.516157848418764, 0.087285996921028, 0.848945555788905, -1.32076496436296, -0.206624472493597, 0.0384537490496661, -1.64901379456232, 0.00972249114872997, 0.381998795844614, 1.01923657104803, 0.202935530782181, -0.889813510533336, -0.380822261972736, -0.265142394236821, 0.642719875389114, -0.435293097251697, 1.19186231409752, -0.731222796943608, 0.141807354004964, -0.503190690187361, -0.0615683920036855, -0.965200188204274, -1.8875707549218, -0.145360959295817, -0.227990245901975, 0.317257684803222, -0.0601827416383581, -1.3714759930137, -1.13109563530008, -0.521419743900848, 0.883552648305685, -0.273458466293969, 0.239695869963475, 0.00893795409190357, -1.01087644731667, -0.327594347683524, 0.752497918090084, -1.53886055692046, -0.15127252491716, 0.417165688316881, 0.580367364725269, -0.296180805911394, 1.96421392132259, 1.29929760275376, 0.018654337394356, 0.283844663311655, 0.5835607512079, 0.207426726914675, -0.136117050138357, 1.03867420020451, 0.981617087838993, -0.429755794899425, -0.439793592290857, 1.73409929102478, 0.453981279141261, -0.0813327646261698, -0.833330461794199, -0.935210759022837, 0.987449306756002, -0.532955886927878, -0.606257606580042, 0.840751593043446, -0.229342520693011, -0.939745027776689, -0.655888898370736, -0.0471966226106494, 0.824091554537458, -1.07262491473119, 0.350411570444895, 0.682187285126274, 0.284374503900471, 1.04489504222055, 0.092454279583159, -0.513441760215648, 0.924178996902974, 0.114616348112289, 0.262716591596937, -0.615358814650848, 0.481499775886303, -0.144626919596748, -0.275712486252055, 0.0723570708988036, 0.81637800953648, -0.112539136718278, 0.957043939599322, 0.471430686172764, 0.412245878792482, -0.776215551200423, 0.114012140950194, 0.680681650378128, -0.447073864376673, 1.00440369907506, 0.730949908253248, 0.816298895080713, 0.131796155186275, 0.482787380336219, 0.711741681542127, -0.283907580554442, -1.32398459761123, 0.0379035168038231, -0.76231616957221, 0.0566274255079344, -0.383779735483127, 0.432881965503507, 0.825416741694984, -0.246603421516066, -0.484494705095186, -0.435917450783736, -0.92275394193025, 1.46590378576233, 0.0946092601374767, 0.142239110610559, -0.461996658454648, -0.466303383682996, 0.548047204640716, 0.0934351410686204, -0.996042810145785, 0.346620395591771, 0.0705671635552156, -1.16296391091913, -0.162154550972467, -1.01393508166724, -1.16498054594965, 0.227893945983562, -0.0783813618255693, 0.375020514416984, 0.590462446604139, -0.332168900678944, 0.608940295809386, 0.440177067038685 });
                }
            }
        }

        public void setupSimulate(double[] weights)
        {
            // Create a board
            this.b = new Board();

            // Assign player
            this.b.player1 = new AlphaBetaBot(1, 3, new int[] { 21 }, weights);// new NNBot(1, new int[] { 21 }, new double[] { 0.03299799516471, -0.857639713379387, 0.387878145970348, 0.0156577529924257, -1.32418078734734, -0.83127629690863, 0.347245136204756, 0.452696991713111, -0.984020506140786, -1.0726306847402, -0.25023112259816, 0.80733424625701, 0.775670472777295, 0.281396572259905, 0.010653234208307, 0.488184464391407, 0.0173993690486065, -0.715289186902945, -0.143924592013436, -0.958393100047667, 0.0302546166490086, 0.121355931331104, -0.0732332915408694, -1.00402563903668, 1.14922425798104, 1.48290392697458, 1.25678327575176, -1.1150036794669, 0.147611378062335, 1.05693584415453, -0.0136508361965653, -0.435447955823246, 0.204274283933581, 0.284173903304233, -0.278649687896785, 0.375081645732318, -0.370778132868362, -0.316486050871427, 0.143505907335088, -2.1345807491497, -0.224240711063259, -0.274109883827209, -0.916426079658058, 0.323210262145479, -0.290717944288961, -0.602932802449415, -0.834827258523939, -1.1609436340681, 0.714911750035785, -1.75398718263208, 0.221048583845165, 0.823544787510086, 0.486273169743956, 0.100030511664241, -0.0622767779567636, -0.522122869744023, 0.160620122640682, -0.10453386004294, -0.0337704731075886, -2.12128528725416, -0.344293864604223, -0.196046119181461, -1.48001240006649, 0.886852323374177, 0.433204839091378, -0.498761553200316, -0.321727841776669, -0.876166307891797, 0.372448473992966, -1.14327340428404, 0.707061878734763, 0.138672406733349, 0.986847714398451, 0.0360248040109991, 0.34727547869425, -0.364666499018048, 0.726565422595742, -0.446615986850399, 1.01270522154528, 0.33378389912368, -0.596735859218396, 0.495013706849429, -1.32380102508413, -0.579144149240639, 0.324603966937682, -2.69793492483345, -1.32117731721195, -1.33701018445986, 0.977682647052073, -1.24339626915445, 0.360095069329299, -1.83637677998579, -0.615183023556686, -1.32313042498339, -0.373488902055421, -0.920663111107733, -0.627983990650617, -0.0660714197745876, -1.1264625386691, 0.287498880777275, -0.346630640256512, -0.534283925096637, -1.04316667446083, 0.617669309311392, -0.714068696235338, -2.2065542284197, 0.471580263679652, 0.506918287489991, 0.96724708819168, -0.875809682940976, 0.64350957860868, -0.240736090224579, 1.92334182766422, 1.23836166003643, -0.015433479689729, 1.02591246484123, -0.139228615974648, 1.75822700083173, -1.95627286597447, 0.404510236766427, -0.00808032253202065, 0.768598106349166, -1.53274335876701, -0.0214519298269658, 0.456759470378402, -0.649068972863755, -0.270774343293521, 0.143232188487068, 1.85310305520105, 1.00805496017358, -0.0988734103035522, -0.687938921008231, -1.28221393901958, -0.0281678310260958, -0.153277870455421, 0.192644390134441, 1.37427205155803, 1.88794261211899, -0.590534192784938, 1.18225072286197, -1.27675262292696, -0.69249097942025, 0.0601269065216776, 1.1514552892891, -0.716397383467479, 0.18936565259442, 1.32417272838027, 0.233514053622965, -0.563617570704603, 0.00130909041096886, 0.6527661532875, -1.08413034297253, 0.884030416088193, 0.925863010425057, -1.47744074928455, -1.14333322429719, -0.645738331319643, -0.260832417179287, 1.11987728910515, -1.04883070187589, -0.0830651320671036, -1.17316623191497, 1.23234158893225, -0.741008062842771, 1.18051161997975, -0.973691901761895, -0.227894732601892, 0.891236223346198 });
            this.b.player2 = new RandomBot(2);
            this.b.player1.B = this.b;
            this.b.player2.B = this.b;
            this.b.currentPlayer = this.b.player1;
        }

        public void process()
        {
            // Execute move (and eat pieces)
            this.b.currentPlayer.makeMove();

            // Convert to King pieces
            this.b.convertToKing();

            // Change sides
            if (this.b.currentPlayer.getId() == 1)
                this.b.currentPlayer = this.b.player2;
            else
                this.b.currentPlayer = this.b.player1;

            this.b.printColored();
        }

        public int playGame()
        {
            int count = 0;
            while (this.b.inGame() & count < 400)
            {
                this.process();
                count++;
                Thread.Sleep(1000);
            }

            return this.b.getWinner();
        }

        public int[] playGames(int number)
        {
            int winner;
            int[] wins = new int[2];
            for (int game = 0; game < number; game++)
            {
                // Play game
                winner = playGame();

                Console.WriteLine("Player " + winner + " won!");

                // Save winner
                if (winner != 0)
                    wins[winner - 1]++;

                // Reset game
                reset(this.b.player1, this.b.player2);
            }

            return wins;
        }

        public void start()
        {
            setup();

            while (true)
            {
                playGames(1);

                // Print end text
                Console.WriteLine(File.ReadAllText(path + "Text/end.txt"));

                // Receive player answers
                int choice;
                Console.Write("Option: ");
                choice = Convert.ToInt32(Console.ReadLine());

                if (choice == 1)
                    reset(this.b.player1, this.b.player2);
                else if (choice == 2)
                    setup();
                else if (choice == 3)
                    System.Environment.Exit(1);
            }
        }

        public void reset(Player p1, Player p2)
        {
            // Create a board
            this.b = new Board();

            // Assign player
            this.b.player1 = p1;
            this.b.player2 = p2;
            this.b.player1.B = this.b;
            this.b.player2.B = this.b;
            this.b.currentPlayer = this.b.player1;

            //Console.WriteLine("Game has been reset");
        }
    }
}