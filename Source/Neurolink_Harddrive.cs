using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Neurolink
{
    public class Neurolink_Harddrive : ThingWithComps {

        public Pawn pawn;

		public Neurolink_Harddrive(Pawn pawn) {
			CopyPawn(pawn);
		}

        public void CopyPawn(Pawn basePawn) {

            NameTriple baseName = basePawn.Name as NameTriple;



            PawnGenerationRequest request = new PawnGenerationRequest(
                basePawn.kindDef,
                faction: basePawn.Faction,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: false,
                fixedGender: basePawn.gender,
                fixedChronologicalAge: basePawn.ageTracker.AgeChronologicalYearsFloat,
                fixedTitle: basePawn.royalty.GetCurrentTitle(basePawn.Faction)
                );

            pawn = PawnGenerator.GeneratePawn(request);

            pawn.Name = new NameTriple(baseName.First, baseName.Nick, baseName.Last);

            pawn.story.adulthood = basePawn.story.adulthood;
            pawn.story.childhood = basePawn.story.childhood;
            pawn.story.traits = basePawn.story.traits;
            pawn.story.title = basePawn.story.title;
            pawn.story.birthLastName = basePawn.story.birthLastName;

            pawn.relations.ClearAllRelations();

            foreach(DirectPawnRelation dpr in basePawn.relations.DirectRelations){
                pawn.relations.AddDirectRelation(dpr.def, dpr.otherPawn);
            } 

            pawn.relations = basePawn.relations;
            pawn.abilities = basePawn.abilities;
            pawn.royalty = basePawn.royalty;
            pawn.royalty.pawn = pawn;
            pawn.guilt = basePawn.guilt;
            pawn.skills = basePawn.skills;
            pawn.thinker = basePawn.thinker;
            pawn.mindState = basePawn.mindState;
            pawn.needs = basePawn.needs;
            pawn.records = basePawn.records;
            pawn.ageTracker = basePawn.ageTracker;
            pawn.kindDef = basePawn.kindDef;
        }
    }
}
