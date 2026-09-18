"use client"
import { TRole } from "@/shared/model/role"
import FormControl from "@mui/material/FormControl"
import Autocomplete from "@mui/material/Autocomplete"
import TextField from "@mui/material/TextField"
import Button from "@mui/material/Button"
import { useCallback, useState } from "react"
import DeleteIcon from "@mui/icons-material/Delete"
import IconButton from "@mui/material/IconButton"
import { TDepartment } from "@/shared/model/department"
import FormGroup from "@mui/material/FormGroup"

interface Props {
	roles: TRole[]
}

interface ISelectedRole {
	role: TRole | null
	department: TDepartment | null
}

const RegistrationRolesField = (props: Props) => {
	const { roles } = props
	const [selectedRoles, setSelectedRoles] = useState<ISelectedRole[]>([])
	const handleAddRole = useCallback(() => {
		setSelectedRoles((state) => [
			...state,
			{ role: null, department: null },
		])
	}, [])
	const handleRemoveRole = useCallback((index: number) => {
		setSelectedRoles((state) => state.filter((_, i) => i !== index))
	}, [])
	return (
		<FormControl>
			<FormGroup>
				{selectedRoles.map((item: ISelectedRole, index: number) => (
					<Autocomplete
						freeSolo
						key={index}
						size="small"
						options={
							roles.length > 0
								? roles
								: [
										{
											uuid: "",
											name: "Нет должностей",
											rights: [],
											roleTypes: [],
											isBase: false,
										},
									]
						}
						getOptionLabel={(option: TRole | string) =>
							typeof option === "string" ? option : option.name
						}
						renderInput={(params) => (
							<TextField
								{...params}
								size="small"
								label="Должность"
							/>
						)}
					/>
				))}
			</FormGroup>
			<Button variant="contained" color="primary" onClick={handleAddRole}>
				Добавить
			</Button>
		</FormControl>
	)
}
export default RegistrationRolesField
