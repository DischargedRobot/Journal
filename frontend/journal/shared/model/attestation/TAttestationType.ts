import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { TAttestationMark } from "./TAttestationMark"

export type TAttestationType = {
	name: string
	attestationMarks: TAttestationMark[]
} & IBaseEntityWithVersion
