import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { TAttestation } from "./TAttestation"

export type TAttestationMark = {
	mark: string
	attestations: TAttestation[]
} & IBaseEntityWithVersion
