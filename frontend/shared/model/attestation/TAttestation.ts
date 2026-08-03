import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { TDate } from "../date"
import { TAttestationType } from "./TAttestationType"
import { TAttestationMark } from "./TAttestationMark"
import { TStudent } from "../student"
import { TDiscipline } from "../discipline"

export type TAttestation = {
	date: TDate
	attestationType: TAttestationType
	attestationMark?: TAttestationMark | null
	student: TStudent
	discipline: TDiscipline
} & IBaseEntityWithVersion
