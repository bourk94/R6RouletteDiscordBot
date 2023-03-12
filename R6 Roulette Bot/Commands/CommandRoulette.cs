using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System.Xml.Serialization;
using System.Timers;
using System.Reflection.Emit;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace R6_Roulette_Bot.Commands
{
    internal class CommandRoulette : BaseCommandModule
    {
        // Attributs
        private static int rerollAtk;
        private static int rerollDef;

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
                    listNumber = randomNumber.Next(0, Program.Attack.size());
                }
                else
                {
                    listNumber = randomNumber.Next(0, Program.Defence.size());
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

            listNumber = randomNumber.Next(0, Program.Penality.size());

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
            message = "L'élément \"" + _nomDefi + "\"  à été ajouté.";
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
            var stream = new FileStream("C:/Users/alexa/Desktop/List_R6_Roulette/" + _nomListe + ".xml", FileMode.Create);
            new XmlSerializer(typeof(BdDefi)).Serialize(stream, _nomBD);
            stream.Close();
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
            await ctx.Channel.SendMessageAsync("**```diff\n-Défis attaque : \n" + Program.Attack.lire(RollChallenge("Attaque", rerollAtk)).ToString() + "\n```" + "```ini\n[Défis défense : ]\n" + Program.Defence.lire(RollChallenge("Defense", rerollDef)).ToString() + "\n```" + "```css\n[Conséquence : ]\n" + Program.Penality.lire(RollPenality()).ToString() + "\n```**").ConfigureAwait(false);
        }

        [Command("attaque"), Aliases("att", "attack", "atk")]
        [Description("Permet de lancer la roulette d'attaque")]
        public async Task RouletteAttack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("**```diff\n-Défis attaque : \n" + Program.Attack.lire(RollChallenge("Attaque", rerollAtk)).ToString() + "\n```" + "```css\n[Conséquence : ]\n" + Program.Penality.lire(RollPenality()).ToString() + "\n```**").ConfigureAwait(false);
        }

        [Command("defense"), Aliases("def", "defence")]
        [Description("Permet de lancer la roulette de défense")]
        public async Task RouletteDefence(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("**```ini\n[Défis défense : ]\n" + Program.Defence.lire(RollChallenge("Defense", rerollDef)).ToString() + "\n```" + "```css\n[Conséquence : ]\n" + Program.Penality.lire(RollPenality()).ToString() + "\n```**").ConfigureAwait(false);
        }

        [Command("ajouterAttaque"), Aliases("addAttaque", "addAttack", "ajouterAttack", "addAtk", "ajouterAtk", "ajAttaque", "ajAttack", "ajAtk", "addAtt", "ajAtt", "ajouterAtt")]
        [Description("Permet d'ajouter un défis d'attaque")]
        public async Task AjouterUneAttack(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Ajouter(Program.Attack, _nom, "AttackList")).ConfigureAwait(false);
        }

        [Command("ajouterDefense"), Aliases("ajouterDefence", "ajouterDef", "ajDef", "ajDefence", "ajDefense", "addDef", "addDefence", "addDefense")]
        [Description("Permet d'ajouter un défis de défense")]
        public async Task AjouterUneDefence(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Ajouter(Program.Defence, _nom, "DefenceList")).ConfigureAwait(false);
        }

        [Command("ajouterConséquence"), Aliases("ajouterPenality", "addPenality", "addConsequence", "addConséquence", "addCon", "addPen", "ajouterPen", "ajouterCon", "ajouterConsequence", "ajCon", "ajPen", "ajConsequence", "ajConséquence")]
        [Description("Permet d'ajouter une conséquence")]
        public async Task AjouterUnePenality(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Ajouter(Program.Penality, _nom, "PenalityList")).ConfigureAwait(false);
        }

        [Command("supprimerAttaque"), Aliases("supprimerAtt", "supprimerAtk", "supprimerAttack", "supAtk", "supAtt", "supAttack", "supAttaque", "delAtt", "delAtk", "delAttack", "delAttaque")]
        [Description("Permet de supprimer un défis d'attaque")]
        public async Task SupprimerUneAttack(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Supprimer(Program.Attack, _nom, "AttackList")).ConfigureAwait(false);
        }

        [Command("supprimerDefense"), Aliases("supprimerDefence", "supprimerDef", "delDef", "delDefence", "delDefense", "supDef", "supDefence", "supDefense")]
        [Description("Permet de supprimer un défis de défense")]
        public async Task SupprimerUneDefence(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Supprimer(Program.Defence, _nom, "DefenceList")).ConfigureAwait(false);
        }

        [Command("supprimerConséquence"), Aliases("supprimerConsequence", "supConsequence", "supCon", "delConsequence", "delCon", "delConséquence", "delPenality", "delPen", "supPenality", "supPen")]
        [Description("Permet de supprimer une conséquence")]
        public async Task SupprimerUnePenality(CommandContext ctx, string _nom)
        {
            await ctx.Channel.SendMessageAsync(Supprimer(Program.Penality, _nom, "PenalityList")).ConfigureAwait(false);
        }

        [Command("listeAttaque"), Aliases("listAttack", "listAttaque", "listAtt", "listeAttack", "listeAtt", "listAtk")]
        [Description("Permet d'afficher la liste des défies d'attaque")]
        public async Task AfficherListAttack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(AfficherListe(Program.Attack)).ConfigureAwait(false);
        }

        [Command("listeDefense"), Aliases("listDefence", "listDefense", "listDef", "listeDef", "listeDefence")]
        [Description("Permet de voir la liste des défies de défense")]
        public async Task AfficherListDefence(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(AfficherListe(Program.Defence)).ConfigureAwait(false);
        }

        [Command("listeConséquence"), Aliases("listPenality", "listConsequence", "listCon", "listeConsequence", "listeCon", "listePenality")]
        [Description("Permet de voir la liste des conséquences")]
        public async Task AfficherListPenality(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(AfficherListe(Program.Penality)).ConfigureAwait(false);
        }

        [Command("R6?")]
        [Description("Tag les gars pour jouer a R6")]
        public async Task AppelR6(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("r6 à soir ? <@277242679933272064> <@291423063701192704> <@372196735411421196> <@334873585040490498>").ConfigureAwait(false);
        }
    }
}
