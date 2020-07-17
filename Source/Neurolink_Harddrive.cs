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
		//Harddrive class stores a Pawn object. Physical properties are not copied to the Pawn object.

        public Pawn pawn;

		public Neurolink_Harddrive(Pawn pawn) {
			CopyPawn(pawn);
			//this.pawn.DeSpawn();
		}

		public void CopyPawn(Pawn basePawn) { //Function called when uploading a pawn's conciousness to a harddrive.

			NameTriple baseName = basePawn.Name as NameTriple;

			//Create a request for PawnGenerator.
			PawnGenerationRequest request = new PawnGenerationRequest(
				basePawn.kindDef,
				faction: basePawn.Faction,
				forceGenerateNewPawn: true,
				canGeneratePawnRelations: false,
				fixedGender: basePawn.gender,
				fixedChronologicalAge: basePawn.ageTracker.AgeChronologicalYearsFloat,
				fixedTitle: basePawn.royalty.GetCurrentTitle(basePawn.Faction)
				);

			//Generate a new pawn using the request we defined.
			pawn = PawnGenerator.GeneratePawn(request);

			//Copy base pawns name to new pawn's.
			pawn.Name = new NameTriple(baseName.First, baseName.Nick, baseName.Last);

			//All instructions below involve individually copying each part of the pawn's bio
			//to the new pawn. Some require different methods of doing so.

			pawn.kindDef = basePawn.kindDef;

			pawn.story.adulthood = basePawn.story.adulthood;
			pawn.story.childhood = basePawn.story.childhood;
			pawn.story.traits = basePawn.story.traits;
			pawn.story.title = basePawn.story.title;
			pawn.story.birthLastName = basePawn.story.birthLastName;

			pawn.relations = new Pawn_RelationsTracker(pawn);
			pawn.relations.ClearAllRelations();

			foreach (DirectPawnRelation dpr in basePawn.relations.DirectRelations){
				pawn.relations.AddDirectRelation(dpr.def, dpr.otherPawn);
			}

			pawn.abilities = basePawn.abilities;
			pawn.abilities.pawn = pawn;

			pawn.royalty = basePawn.royalty;
			pawn.royalty.pawn = pawn;

			pawn.skills = new Pawn_SkillTracker(pawn);
			pawn.skills.skills = basePawn.skills.skills;

			pawn.thinker = basePawn.thinker;
			pawn.thinker.pawn = pawn;

			
			pawn.mindState = basePawn.mindState;
			pawn.mindState.pawn = pawn;

			pawn.needs = new Pawn_NeedsTracker(pawn);
		   
			pawn.records = basePawn.records;
			pawn.records.pawn = pawn;

			pawn.ageTracker = new Pawn_AgeTracker(pawn);
			pawn.ageTracker.AgeBiologicalTicks = 0;
			pawn.ageTracker.AgeChronologicalTicks = basePawn.ageTracker.AgeChronologicalTicks;
			
		}
	}
}
