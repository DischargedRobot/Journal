import { TStudent } from "./TStudent"

export const formatStudentShortName = (
	student: Pick<TStudent, "lastName" | "firstName" | "patronymic">,
) => {
	const parts = [student.lastName]

	if (student.firstName) {
		parts.push(`${student.firstName[0]}.`)
	}
	if (student.patronymic) {
		parts.push(`${student.patronymic[0]}.`)
	}

	return parts.join(" ")
}
