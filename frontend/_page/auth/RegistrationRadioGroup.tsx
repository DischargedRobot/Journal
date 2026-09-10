import FormControlLabel from "@mui/material/FormControlLabel"
import Radio from "@mui/material/Radio"
import RadioGroup from "@mui/material/RadioGroup"
import { memo, useState } from "react"
import { FormValues } from "./fields"
import useRegistrationFormStore from "./model/useRegistrationFormStore"
import FormLabel from "@mui/material/FormLabel"
import FormControl from "@mui/material/FormControl"
import FormHelperText from "@mui/material/FormHelperText"
import TextField from "@mui/material/TextField"
import { TGroupResponseDto } from "@/shared/api/group"
import { TDepartmentResponseDto } from "@/shared/api/department"
import Autocomplete from "@mui/material/Autocomplete"
import RegistrationAutocomplete from "./RegistrationAutocomplete"

interface Props {
	groups: TGroupResponseDto[]
	departments: TDepartmentResponseDto[]
}

const RegistrationRadioGroup = (props: Props) => {
	const { groups, departments } = props

	console.log("RegistrationRadioGroup render")

	const selectedGroup = useRegistrationFormStore(
		(state) => state.formValues.group,
	)
	const selectedDepartment = useRegistrationFormStore(
		(state) => state.formValues.department,
	)
	const personRole = useRegistrationFormStore(
		(state) => state.formValues.personRole,
	)
	const updateField = useRegistrationFormStore((state) => state.updateField)

	return (
		<>
			<FormControl
				component="fieldset"
				error={personRole.error.length > 0}
			>
				<FormLabel component="legend">Роль</FormLabel>
				<RadioGroup
					row
					value={personRole.value}
					onChange={(_, value) => {
						updateField("personRole", value)
					}}
				>
					<FormControlLabel
						value="STUDENT"
						control={<Radio />}
						label="Студент"
					/>
					<FormControlLabel
						value="TEACHER"
						control={<Radio />}
						label="Преподаватель"
					/>
				</RadioGroup>
				<FormHelperText>
					{personRole?.error.length > 0 ? personRole.error : " "}
				</FormHelperText>
			</FormControl>

			{personRole.value === "STUDENT" && (
				<RegistrationAutocomplete
					items={departments}
					fieldName="department"
					label={groups.length > 0 ? "Группа" : "Групп нет"}
				/>
			)}

			{personRole.value === "TEACHER" && (
				<RegistrationAutocomplete
					items={departments}
					fieldName="department"
					label={departments.length > 0 ? "Кафедра" : "Кафедр нет"}
				/>
			)}
		</>
	)
}

export default memo(RegistrationRadioGroup)
