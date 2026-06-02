import {
	TAttestation,
	TAttestationMark,
	TAttestationType,
} from "@/shared/model/attestation"
import { TDate } from "@/shared/model/date"
import { TDiscipline } from "@/shared/model/discipline"
import { TStudent } from "@/shared/model/student"

type CreateMockAttestationOptions = {
	date?: TDate
	attestationTypeName?: string
}

export const createMockAttestation = (
	student: TStudent,
	discipline: TDiscipline,
	mark: string,
	options: CreateMockAttestationOptions = {},
): TAttestation => {
	const attestationMark: TAttestationMark = {
		uuid: `attestation-mark-${student.uuid}-${discipline.uuid}`,
		version: 1,
		mark,
		attestations: [],
	}

	const attestationType: TAttestationType = {
		uuid: `attestation-type-${discipline.uuid}`,
		version: 1,
		name: options.attestationTypeName ?? "Аттестация",
		attestationMarks: [attestationMark],
	}

	const attestation: TAttestation = {
		uuid: `attestation-${student.uuid}-${discipline.uuid}`,
		version: 1,
		date: options.date ?? "2024-12-20 10:00",
		attestationType,
		attestationMark,
		student,
		discipline,
	}

	attestationMark.attestations = [attestation]

	return attestation
}

export const setStudentAttestationMark = (
	student: TStudent,
	discipline: TDiscipline,
	mark: string,
	options?: CreateMockAttestationOptions,
) => {
	student.attestations.set(
		discipline.uuid,
		createMockAttestation(student, discipline, mark, options),
	)
}
