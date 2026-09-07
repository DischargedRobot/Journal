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
				<Autocomplete
					freeSolo
					options={groups
						.map((g) => g.code)
						.filter(
							(g, index, self) =>
								index ===
								self.findIndex(
									(findingGroup) => findingGroup == g,
								),
						)}
					value={selectedGroup.value}
					onChange={(_, newValue) => {
						// Выбрали группу из списка
						updateField("group", newValue ?? "")
					}}
					onInputChange={(_, newInputValue) => {
						// Пользователь вводит своё значение
						updateField("group", newInputValue)
					}}
					disabled={groups.length === 0}
					renderInput={(params) => (
						<TextField
							{...params}
							variant="outlined"
							required
							label={groups.length > 0 ? "Группа" : "Групп нет"}
							size="small"
							error={!!selectedGroup.error}
							helperText={
								selectedGroup.error.length > 0
									? selectedGroup.error
									: " "
							}
						/>
					)}
				/>
			)}

			{personRole.value === "TEACHER" && (
				<Autocomplete
					freeSolo
					options={departments
						.map((department) => department.code)
						.filter(
							(code, index, self) =>
								index ===
								self.findIndex((item) => item === code),
						)}
					value={selectedDepartment.value}
					onChange={(_, newValue) => {
						console.log(newValue, "sadas")
						updateField("department", newValue ?? "")
					}}
					onInputChange={(_, newInputValue) => {
						console.log(newInputValue, "sadas2")

						updateField("department", newInputValue)
					}}
					disabled={departments.length === 0}
					renderInput={(params) => (
						<TextField
							{...params}
							variant="outlined"
							required
							label={
								departments.length > 0
									? "Кафедра"
									: "Кафедр нет"
							}
							size="small"
							error={!!selectedDepartment.error}
							helperText={
								selectedDepartment.error.length > 0
									? selectedDepartment.error
									: " "
							}
						/>
					)}
				/>
			)}
		</>
	)
}

export default memo(RegistrationRadioGroup)
