"use client"

import MuiCheckbox, { CheckboxProps as MuiCheckboxProps } from "@mui/material/Checkbox"
import CheckboxRectCheckedIcon from "./CheckboxRectCheckedIcon"
import CheckboxRectUncheckedIcon from "./CheckboxRectUncheckedIcon"

export type CheckboxRectProps = MuiCheckboxProps

const iconSx = {
	padding: 0,
	width: 16,
	height: 16,
} as const

const CheckboxRect = ({ sx, ...props }: CheckboxRectProps) => (
	<MuiCheckbox
		{...props}
		icon={<CheckboxRectUncheckedIcon />}
		checkedIcon={<CheckboxRectCheckedIcon />}
		sx={[
			iconSx,
			...(Array.isArray(sx) ? sx : [sx]),
		]}
	/>
)

export default CheckboxRect
