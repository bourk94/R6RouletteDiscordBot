using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Xml.Serialization;

namespace R6_Roulette_Bot.Commands
{
    internal class CommandRoulette : BaseCommandModule
    {
        // Attributs
        private static int rerollAtk;
        private static int rerollDef;
        private string attackList = "AttackList";
        private string defenceList = "DefenceList";
        private string penalityList = "PenalityList";
        private BdDefi dbAttack = Program.Attack;
        private BdDefi dbDefence = Program.Defence;
        private BdDefi dbPenality = Program.Penality;
        private VoiceDetection voiceDetection;

        public CommandRoulette()
        {
            this.voiceDetection = new VoiceDetection(this);
        }

        // Méthodes privées

        // Méthode qui va choisir un défi d'attaque ou de défense de manière aléatoire
        private int RollChallenge(string _nomPhase, int _reroll)
        {
            Random randomNumber = new Random();
            int listNumber;
            do
            {
                if (_nomPhase == "Attaque")
                {
                    listNumber = randomNumber.Next(0, dbAttack.size());
                }
                else
                {
                    listNumber = randomNumber.Next(0, dbDefence.size());
                }
            } while (listNumber == _reroll);

            if (_nomPhase == "Attaque")
            {
                rerollAtk = listNumber;
            }
            else
            {
                rerollDef = listNumber;
            }
            return listNumber;
        }

        // Méthode qui va choisir une penalité de manière aléatoire
        private int RollPenality()
        {
            Random randomNumber = new Random();
            int listNumber;

            listNumber = randomNumber.Next(0, dbPenality.size());

            return listNumber;
        }

        // Méthode qui vérifie si le défi existe déjà ou si le champ est vide
        private string VerificationDefi(BdDefi _nomBD, string _nomDefi)
        {
            string nonExistant = "true";

            if (string.IsNullOrEmpty(_nomDefi) || string.IsNullOrWhiteSpace(_nomDefi))
            {
                nonExistant = "vide";
                return nonExistant;
            }

            foreach (var defi in _nomBD)
            {
                if (defi.ToString().ToLower().Equals(_nomDefi.ToLower()))
                {
                    nonExistant = "false";
                    return nonExistant;
                }
            }
            return nonExistant;
        }

        // Méthode pour ajouter un defi ou une pénalité
        private string Ajouter(BdDefi _nomBD, string _nomDefi, string _nomListe)
        {
            string message = "";
            Defi defie = new Defi();
            defie.Name = _nomDefi;

            if (VerificationDefi(_nomBD, _nomDefi) == "false")
            {
                message = "Le défi ou la pénalité est déjà présent dans la liste.";
                return message;
            }

            if (VerificationDefi(_nomBD, _nomDefi) == "vide")
            {
                message = "Un défi ou une pénalité ne peut pas être vide.";
                return message;
            }

            _nomBD.Add(defie);
            SauvegarderListe(_nomListe, _nomBD);
            message = "L'élément \"" + _nomDefi + "\" à été ajouté.";
            return message;
        }

        // Méthode pour supprimer un defi ou une pénalité
        private string Supprimer(BdDefi _nomBD, string _nomDefi, string _nomListe)
        {
            bool present = false;
            string message = "";
            for (int i = 0; i < _nomBD.size(); i++)
            {
                Defi defi = _nomBD.lire(i);
                if (defi.Name.ToLower().Equals(_nomDefi.ToLower()))
                {
                    _nomBD.supprimer(i);
                    SauvegarderListe(_nomListe, _nomBD);
                    message = "L'élément \"" + _nomDefi + "\" à été supprimé.";
                    present = true;
                }
            }

            if (!present)
            {
                message = "L'élément \"" + _nomDefi + "\" n'existe pas.";
                return message;
            }
            return message;
        }

        // Méthode pour sauvegarder les listes des défis et pénalité
        private void SauvegarderListe(string _nomListe, BdDefi _nomBD)
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string xmlFilePath = Path.Combine(projectDirectory, "List_R6_Roulette", _nomListe + ".xml");

            using (var stream = new FileStream(xmlFilePath, FileMode.Create))
            {
                new XmlSerializer(typeof(BdDefi)).Serialize(stream, _nomBD);
            }
        }


        // Méthode pour afficher les listes
        private string AfficherListe(BdDefi _nomBD)
        {
            string message = "";

            foreach (var defi in _nomBD)
            {
                message += defi + "\n";
            }
            return message;
        }


        // Méthodes publiques
        // Méthodes pour lancer les commandes

        [Command("strat"), Aliases("strats", "str", "strt")]
        [Description("Permet de lancer la roulette d'attaque et de défense simultanément")]
        public async Task RouletteStrat(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("**```diff\n-Défis attaque : \n" + dbAttack.lire(RollChallenge("Attaque", rerollAtk)).ToString() + "\n```" + "```ini\n[Défis défense : ]\n" + dbDefence.lire(RollChallenge("Defense", rerollDef)).ToString() + "\n```" + "```css\n[Conséquence : ]\n" + dbPenality.lire(RollPenality()).ToString() + "\n```**").ConfigureAwait(false);
        }

        [Command("attaque"), Aliases("att", "attack", "atk")]
        [Description("Permet de lancer la roulette d'attaque")]
        public async Task RouletteAttack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("**```diff\n-Défis attaque : \n" + dbAttack.lire(RollChallenge("Attaque", rerollAtk)).ToString() + "\n```" + "```css\n[Conséquence : ]\n" + dbPenality.lire(RollPenality()).ToString() + "\n```**").ConfigureAwait(false);
        }

        [Command("defense"), Aliases("def", "defence")]
        [Description("Permet de lancer la roulette de défense")]
        public async Task RouletteDefence(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("**```ini\n[Défis défense : ]\n" + dbDefence.lire(RollChallenge("Defense", rerollDef)).ToString() + "\n```" + "```css\n[Conséquence : ]\n" + dbPenality.lire(RollPenality()).ToString() + "\n```**").ConfigureAwait(false);
        }

        [Command("ajouterAttaque"), Aliases("addAttaque", "addAttack", "ajouterAttack", "addAtk", "ajouterAtk", "ajAttaque", "ajAttack", "ajAtk", "addAtt", "ajAtt", "ajouterAtt")]
        [Description("Permet d'ajouter un défis d'attaque")]
        public async Task AjouterUneAttack(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Ajouter(dbAttack, _nom, attackList)).ConfigureAwait(false);
        }

        [Command("ajouterDefense"), Aliases("ajouterDefence", "ajouterDef", "ajDef", "ajDefence", "ajDefense", "addDef", "addDefence", "addDefense")]
        [Description("Permet d'ajouter un défis de défense")]
        public async Task AjouterUneDefence(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Ajouter(dbDefence, _nom, defenceList)).ConfigureAwait(false);
        }

        [Command("ajouterConséquence"), Aliases("ajouterPenality", "addPenality", "addConsequence", "addConséquence", "addCon", "addPen", "ajouterPen", "ajouterCon", "ajouterConsequence", "ajCon", "ajPen", "ajConsequence", "ajConséquence")]
        [Description("Permet d'ajouter une conséquence")]
        public async Task AjouterUnePenality(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Ajouter(dbPenality, _nom, penalityList)).ConfigureAwait(false);
        }

        [Command("supprimerAttaque"), Aliases("supprimerAtt", "supprimerAtk", "supprimerAttack", "supAtk", "supAtt", "supAttack", "supAttaque", "delAtt", "delAtk", "delAttack", "delAttaque")]
        [Description("Permet de supprimer un défis d'attaque")]
        public async Task SupprimerUneAttack(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Supprimer(dbAttack, _nom, attackList)).ConfigureAwait(false);
        }

        [Command("supprimerDefense"), Aliases("supprimerDefence", "supprimerDef", "delDef", "delDefence", "delDefense", "supDef", "supDefence", "supDefense")]
        [Description("Permet de supprimer un défis de défense")]
        public async Task SupprimerUneDefence(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Supprimer(dbDefence, _nom, defenceList)).ConfigureAwait(false);
        }

        [Command("supprimerConséquence"), Aliases("supprimerConsequence", "supConsequence", "supCon", "delConsequence", "delCon", "delConséquence", "delPenality", "delPen", "supPenality", "supPen")]
        [Description("Permet de supprimer une conséquence")]
        public async Task SupprimerUnePenality(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Supprimer(dbPenality, _nom, penalityList)).ConfigureAwait(false);
        }

        [Command("listeAttaque"), Aliases("listAttack", "listAttaque", "listAtt", "listeAttack", "listeAtt", "listAtk")]
        [Description("Permet d'afficher la liste des défies d'attaque")]
        public async Task AfficherListAttack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(AfficherListe(dbAttack)).ConfigureAwait(false);
        }

        [Command("listeDefense"), Aliases("listDefence", "listDefense", "listDef", "listeDef", "listeDefence")]
        [Description("Permet de voir la liste des défies de défense")]
        public async Task AfficherListDefence(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(AfficherListe(dbDefence)).ConfigureAwait(false);
        }

        [Command("listeConséquence"), Aliases("listPenality", "listConsequence", "listCon", "listeConsequence", "listeCon", "listePenality")]
        [Description("Permet de voir la liste des conséquences")]
        public async Task AfficherListPenality(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(AfficherListe(dbPenality)).ConfigureAwait(false);
        }

        [Command("r6?"), Aliases("r6")]
        [Description("Tag les gars pour jouer a R6")]
        public async Task AppelR6(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("<@&1072650837630931076> REEEEEEEEEEEEEEEEEEEEEEEEEEE").ConfigureAwait(false);
        }

        [Command("join")]
        [Description("Le bot rejoint le salon")]
        public async Task JoinChannel(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
            {
                throw new InvalidOperationException("Already connected in this guild.");
            }

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
            {
                throw new InvalidOperationException("You need to be in a voice channel.");
            }

            vnc = await vnext.ConnectAsync(chn);

            await ctx.RespondAsync($"Connecté à {chn.Name}").ConfigureAwait(false);

            voiceDetection.SetCommandContext(ctx);

            vnc.VoiceReceived += voiceDetection.ReceiveHandler;
        }

        [Command("leave")]
        [Description("Le bot quitte le salon")]
        public async Task LeaveChannel(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
            {
                throw new InvalidOperationException("Not connected in this guild.");
            }

            vnc.Disconnect();

            vnc.VoiceReceived -= voiceDetection.ReceiveHandler;

            await ctx.RespondAsync("Déconnecté").ConfigureAwait(false);
        }
    }
}
